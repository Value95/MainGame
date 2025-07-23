using System.Collections.ObjectModel;
using System.ComponentModel;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    class MatchmakerConfigResource : IDeploymentItem, ITypedItem
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public float Progress { get; }
        
        public DeploymentStatus Status { get; set; }
        
        public ObservableCollection<AssetState> States { get; }

        public IMatchmakerConfig Content { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public string Type => Content?.Type.ToString() ?? "Undefined";

        public override string ToString()
        {
            return $"[{Type}] '{Name}' in {Path}";
        }
    }
}
