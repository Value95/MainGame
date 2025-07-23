using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Matchmaker.Authoring.Core.Model;
using Unity.Services.Matchmaker.Authoring.Core.ConfigApi;
using Unity.Services.Matchmaker.Authoring.Core.Fetch;
using Unity.Services.Matchmaker.Authoring.Core.Parser;
using DeploymentStatus = Unity.Services.Matchmaker.Authoring.Core.ConfigApi.DeploymentStatus;

namespace Unity.Services.Matchmaker.Authoring.Core.Deploy
{
    class MatchmakerDeployHandler : IMatchmakerDeployHandler
    {
        readonly IConfigApiClient m_configApiClient;
        readonly IMatchmakerConfigParser m_configParser;
        readonly IDeepEqualityComparer m_deepEqualityComparer;

        public MatchmakerDeployHandler(IConfigApiClient configApiClient,
            IMatchmakerConfigParser configParser,
            IDeepEqualityComparer deepEqualityComparer)
        {
            m_configApiClient = configApiClient;
            m_configParser = configParser;
            m_deepEqualityComparer = deepEqualityComparer;
        }

        public async Task<DeployResult> DeployAsync(
            IReadOnlyList<string> localResourcePaths,
            MultiplayResources availableMultiplayResources,
            bool reconcile,
            bool dryRun,
            CancellationToken ct = default)
        {
            var result = new DeployResult();

            // Parsing
            var parseResult = await m_configParser.Parse(localResourcePaths, ct);
            result.Failed.AddRange(parseResult.failed);
            if (parseResult.parsed.Count == 0 && !reconcile)
                return result;
            
            var (configExist, originalEnvironmentConfig) = await m_configApiClient.GetEnvironmentConfig(ct);
            // Error don't matter on list queue cause we want to fix the config by deploying
            var originalQueues = (await m_configApiClient.ListQueues(ct)).Select(q => q.Item1).ToList();

            // envConfigFile.Content should be EnvironmentConfig (handled by parser)
            // If no envConfig to deploy, take a default envConfig
            var envConfigFile = parseResult.parsed.Find(p => p.Content is EnvironmentConfig);
            var queueConfigFiles = parseResult.parsed.Where(p => p.Content is QueueConfig).ToList();
            var targetEnvConfig = envConfigFile?.Content as EnvironmentConfig ?? new EnvironmentConfig();
    
            var envConfigCreated = false;
            // Deploy envConfig first if it's the first deployment
            List<ErrorResponse> errors;
            if (!configExist)
            {
                // Don't deploy default queue name as part of original upsert since the reference isn't deployed yet
                var initConfig = new EnvironmentConfig
                {
                    Enabled = targetEnvConfig.Enabled
                };
                errors = await m_configApiClient.UpsertEnvironmentConfig(initConfig, dryRun, ct);
                if(errors.Count != 0)
                {
                    if (envConfigFile != null)
                    {
                        envConfigFile.Status = DeploymentStatus.Get("Failed to upsert Environment Config",  errors);
                        result.Failed.Add(envConfigFile);
                    }
                    return result;
                }
                envConfigCreated = true;
            }

            
            // Queues reconcile
            if (reconcile)
            {
                var targetQueueNames = queueConfigFiles.Select(q =>
                {
                    // q.Content should be QueueConfig (handled by parser)
                    var queueConfig = q.Content as QueueConfig;
                    return queueConfig?.Name?.ToString();
                }).ToList();

                foreach (var originalQueue in originalQueues)
                {
                    if (targetQueueNames.Contains(originalQueue.Name.ToString()))
                        continue;

                    var reconcileQueueResult = new MatchmakerConfigResource
                    {
                        Name = originalQueue.Name.ToString(),
                        Content = originalQueue,
                        Status = DeploymentStatus.Get("Deleted", "queue", dryRun , originalQueue.Name.ToString()),
                        Path = ""
                    };
                    await m_configApiClient.DeleteQueue(originalQueue.Name, dryRun, ct);
                    result.Deleted.Add(reconcileQueueResult);
                    result.Authored.Add(reconcileQueueResult);
                }
            }

            foreach (var queueConfigFile in queueConfigFiles)
            {
                // queueConfigFile.Content should be QueueConfig (handled by parser)
                var queueConfig = queueConfigFile.Content as QueueConfig;
                if (queueConfig == null)
                    continue;

                var originalQueue = originalQueues.Find(q => q.Name.Equals(queueConfig.Name));
                if (originalQueue != null && m_deepEqualityComparer.IsDeepEqual(queueConfig, originalQueue))
                {
                    queueConfigFile.Status = DeploymentStatus.Get("Updated (no changes)", "queue", dryRun , queueConfig.Name.ToString());
                    result.Updated.Add(queueConfigFile);
                    result.Authored.Add(queueConfigFile);
                    continue;
                }
                errors = await m_configApiClient.UpsertQueue(queueConfig, availableMultiplayResources, dryRun, ct);
                if(errors.Count != 0)
                {
                    queueConfigFile.Status = DeploymentStatus.Get($"Failed to update queue {queueConfig.Name}", errors);
                    result.Failed.Add(queueConfigFile);
                    continue;
                }

                if (originalQueue == null)
                {
                    queueConfigFile.Status = DeploymentStatus.Get("Created", "queue", dryRun , queueConfig.Name.ToString());
                    result.Created.Add(queueConfigFile);
                }
                else
                {
                    var poolUpdateDetails  = PoolUpdateResult.GetPoolDeployResultFromQueues(originalQueue, queueConfig, m_deepEqualityComparer, dryRun);
                    queueConfigFile.Status = DeploymentStatus.Get("Updated", "queue", dryRun , queueConfig.Name.ToString(), SeverityLevel.Info, poolUpdateDetails);
                    result.Updated.Add(queueConfigFile);
                }
                
                result.Authored.Add(queueConfigFile);
            }

            if (envConfigFile == null)
            {
                return result;
            }
            if (m_deepEqualityComparer.IsDeepEqual(targetEnvConfig, originalEnvironmentConfig))
            {
                envConfigFile.Status = DeploymentStatus.Get("Updated (no changes)", "environmentConfig", dryRun);
                result.Updated.Add(envConfigFile);
                result.Authored.Add(envConfigFile);
                return result;
            }
            
            errors = await m_configApiClient.UpsertEnvironmentConfig(targetEnvConfig, dryRun, ct);
            if (errors.Count != 0)
            {
                envConfigFile.Status = DeploymentStatus.Get("Failed to update EnvironmentConfig", errors);
                result.Failed.Add(envConfigFile);
            }
            else
            {
                if (envConfigCreated)
                {
                    envConfigFile.Status = DeploymentStatus.Get("Created", "environmentConfig", dryRun);
                    result.Created.Add(envConfigFile);
                }
                else
                {
                    envConfigFile.Status = DeploymentStatus.Get("Updated", "environmentConfig", dryRun);
                    result.Updated.Add(envConfigFile);
                }
                result.Authored.Add(envConfigFile);
            }
            return result;
        }


    }
}
