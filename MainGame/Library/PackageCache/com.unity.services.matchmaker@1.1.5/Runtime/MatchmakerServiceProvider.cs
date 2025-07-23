using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Apis.Backfill;
using Unity.Services.Matchmaker.Apis.Tickets;
using Unity.Services.Matchmaker.Http;
using Unity.Services.Core.Internal;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Analytics.Internal;

namespace Unity.Services.Matchmaker
{
    internal class MatchmakerServiceProvider : IInitializablePackage
    {
        const string k_CloudEnvironmentKey = "com.unity.services.core.cloud-environment";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Register()
        {
            // Pass an instance of this class to Core
            var generatedPackageRegistry = CoreRegistry.Instance.RegisterPackage(new MatchmakerServiceProvider());

            // And specify what components it requires, or provides.
            generatedPackageRegistry.DependsOn<IAccessToken>();
            generatedPackageRegistry.DependsOn<IProjectConfiguration>();
            generatedPackageRegistry.OptionallyDependsOn<ICloudProjectId>();
            generatedPackageRegistry.OptionallyDependsOn<IEnvironmentId>();
            generatedPackageRegistry.OptionallyDependsOn<IInstallationId>();
            generatedPackageRegistry.OptionallyDependsOn<IAnalyticsStandardEventComponent>();
        }

        public Task Initialize(CoreRegistry registry)
        {
            var httpClient = new HttpClient();

            var accessTokenMatchmaker = registry.GetServiceComponent<IAccessToken>();
            var projectConfiguration = registry.GetServiceComponent<IProjectConfiguration>();

            if (accessTokenMatchmaker != null)
            {
                MatchmakerServiceSdk.Instance =
                    new InternalMatchmakerServiceSdk(
                        httpClient,
                        projectConfiguration.GetString(k_CloudEnvironmentKey),
                        registry.GetServiceComponent<IAccessToken>());
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// InternalMatchmakerService
    /// </summary>
    internal class InternalMatchmakerServiceSdk : IMatchmakerServiceSdk
    {
        /// <summary>
        /// Constructor for InternalMatchmakerService
        /// </summary>
        /// <param name="httpClient">The HttpClient for InternalMatchmakerService.</param>
        /// <param name="cloudEnvironment">The current environment</param>
        /// <param name="accessToken">The Authentication token for the service.</param>
        public InternalMatchmakerServiceSdk(HttpClient httpClient, string cloudEnvironment = "production", IAccessToken accessToken = null)
        {

            BackfillApi = new BackfillApiClient(httpClient, accessToken);

            TicketsApi = new TicketsApiClient(httpClient, accessToken);

            AccessToken = accessToken;

            Configuration = new Configuration(GetBasePath(cloudEnvironment), 10, 4, null);
        }

        /// <summary> Instance of IBackfillApiClient interface</summary>
        public IBackfillApiClient BackfillApi { get; set; }

        /// <summary> Instance of ITicketsApiClient interface</summary>
        public ITicketsApiClient TicketsApi { get; set; }

        /// <summary> Instance of AccessToken interface</summary>
        public IAccessToken AccessToken { get; set; }

        /// <summary> Configuration properties for the service.</summary>
        public Configuration Configuration { get; set; }

        private string GetBasePath(string cloudEnvironment)
        {
            if (cloudEnvironment == "staging")
            {
                return "https://matchmaker-stg.services.api.unity.com";
            }
            return "https://matchmaker.services.api.unity.com";
        }
    }
}
