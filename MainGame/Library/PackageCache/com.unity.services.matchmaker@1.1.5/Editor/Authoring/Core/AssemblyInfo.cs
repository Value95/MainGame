using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.Matchmaker.Authoring.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Services.Cli.Matchmaker")]
[assembly: InternalsVisibleTo("Unity.Services.Cli.Matchmaker.UnitTest")]
[assembly: InternalsVisibleTo("InternalsVisible.DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// Needed to enable record types
namespace Unity.Services.Matchmaker.Authoring.Core
{
    static class IsExternalInit {}
}
