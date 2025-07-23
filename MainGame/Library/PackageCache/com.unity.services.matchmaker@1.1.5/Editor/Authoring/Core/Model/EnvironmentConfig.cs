using System.Runtime.Serialization;

namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    class EnvironmentConfig : IMatchmakerConfig
    {
        [IgnoreDataMember]
        public IMatchmakerConfig.ConfigType Type { get; set; } = IMatchmakerConfig.ConfigType.EnvironmentConfig;
        
        [DataMember(Name = "$schema")]
        public string Schema = "https://ugs-config-schemas.unity3d.com/v1/matchmaker/matchmaker-environment-config.schema.json";

        [DataMember(IsRequired = true)] public bool Enabled { get; set; }

        [DataMember(IsRequired = false)] public QueueName DefaultQueueName { get; set; } = new();

    }
}
