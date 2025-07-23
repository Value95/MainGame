namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    interface IMatchmakerConfig
    {
        public ConfigType Type { get; set; }

        public enum ConfigType
        {
            Unspecified,
            EnvironmentConfig,
            QueueConfig
        }
    }
}
