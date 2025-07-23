using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Authoring.Core.Model;

namespace Unity.Services.Matchmaker.Authoring.Core.Deploy
{
    interface IMatchmakerDeployHandler
    {
        Task<DeployResult> DeployAsync(IReadOnlyList<string> filePaths, MultiplayResources availableMultiplayResources, bool reconcile, bool dryRun, CancellationToken ct = default);
    }
}
