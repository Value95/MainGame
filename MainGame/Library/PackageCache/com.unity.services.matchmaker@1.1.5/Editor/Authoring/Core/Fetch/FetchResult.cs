using System.Collections.Generic;
using Unity.Services.Matchmaker.Authoring.Core.Model;

namespace Unity.Services.Matchmaker.Authoring.Core.Fetch
{
    class FetchResult
    {
        
        public string AbortMessage { get; set; } = "";

        public List<MatchmakerConfigResource> Created { get; } = new();

        public List<MatchmakerConfigResource> Updated { get; } = new();

        public List<MatchmakerConfigResource> Deleted { get; } = new();

        public List<MatchmakerConfigResource> Authored { get; } = new();

        public List<MatchmakerConfigResource> Failed { get; } = new();
    }
}
