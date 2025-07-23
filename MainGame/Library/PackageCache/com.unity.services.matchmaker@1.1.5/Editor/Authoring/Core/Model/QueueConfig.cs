using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    class QueueConfig : IMatchmakerConfig
    {
        [IgnoreDataMember]
        public IMatchmakerConfig.ConfigType Type { get; set; } = IMatchmakerConfig.ConfigType.QueueConfig;
        
        [DataMember(Name = "$schema")]
        public string Schema = "https://ugs-config-schemas.unity3d.com/v1/matchmaker/matchmaker-queue.schema.json";

        [DataMember(IsRequired = true)] public QueueName Name { get; set; }

        [DataMember(IsRequired = true)] public bool Enabled { get; set; } = true;

        [DataMember(IsRequired = true)] public int MaxPlayersPerTicket { get; set; }

        [DataMember(IsRequired = false)] public BasePoolConfig DefaultPool { get; set; }

        [DataMember(IsRequired = false)] public List<FilteredPoolConfig> FilteredPools { get; set; } = new();
    }
    
     internal class PoolConfig
        {
            internal PoolConfig() { }

            internal PoolConfig(PoolConfig poolConfig)
            {
                if(poolConfig == null) return;
                Enabled = poolConfig.Enabled;
                Name = poolConfig.Name;
                TimeoutSeconds = poolConfig.TimeoutSeconds;
                MatchLogic = poolConfig.MatchLogic;
                MatchHosting = poolConfig.MatchHosting;
            }

            [DataMember(IsRequired = true)] public PoolName Name { get; set; }

            [DataMember(IsRequired = true)] public bool Enabled { get; set; }
            
            [DataMember(IsRequired = true)] public int TimeoutSeconds { get; set; }

            [DataMember(IsRequired = true)]
            public MatchLogicRulesConfig MatchLogic { get; set; }

            [DataMember(IsRequired = true)] public IMatchHostingConfig MatchHosting { get; set; }
        }

        internal class BasePoolConfig : PoolConfig
        {
            internal BasePoolConfig() { }

            internal BasePoolConfig(PoolConfig poolConfig, List<PoolConfig> variants = null) : base(poolConfig)
            {
                Variants = variants ?? new();
            }

            [DataMember(IsRequired = false)] public List<PoolConfig> Variants { get; set; } = new();
        }

        internal class FilteredPoolConfig : BasePoolConfig
        {
            internal FilteredPoolConfig() { }

            internal FilteredPoolConfig(PoolConfig poolConfig, List<Filter> filters = null,
                List<PoolConfig> variants = null) : base(poolConfig, variants)
            {
                Filters = filters ?? new();
            }

            [DataMember(IsRequired = true)] public List<Filter> Filters { get; set; } = new();

            internal class Filter
            {
                [DataMember(IsRequired = true)] public string Attribute { get; set; }

                [DataMember(IsRequired = true)] public FilterOperator Operator { get; set; } = FilterOperator.Equal;

                [DataMember(IsRequired = true)] public JsonObject Value { get; set; }

                public enum FilterOperator
                {
                    Equal,
                    NotEqual,
                    LessThan,
                    GreaterThan
                }
            }
        }
        
        internal class MatchLogicRulesConfig
        {
            [DataMember(IsRequired = true)] public RuleBasedMatchDefinition MatchDefinition { get; set; }
            
            [DataMember(IsRequired = true)] public string Name { get; set; }
            
            [DataMember(IsRequired = true)] public bool BackfillEnabled { get; set; }
        }

        internal interface IMatchHostingConfig
        {
            public MatchHostingType Type { get; set; }

            public enum MatchHostingType
            {
                Invalid,
                Multiplay,
                MatchId
            }
        }

        internal class MultiplayConfig : IMatchHostingConfig
        {
            [DataMember(IsRequired = true)]
            public IMatchHostingConfig.MatchHostingType Type { get; set; } = IMatchHostingConfig.MatchHostingType.Multiplay;

            [DataMember(IsRequired = false)] public string FleetName { get; set; }

            [DataMember(IsRequired = false)] public string BuildConfigurationName { get; set; }

            [DataMember(IsRequired = false)] public string DefaultQoSRegionName { get; set; }
        }

        internal class MatchIdConfig : IMatchHostingConfig
        {
            [DataMember(IsRequired = true)]
            public IMatchHostingConfig.MatchHostingType Type { get; set; } =
                IMatchHostingConfig.MatchHostingType.MatchId;
        }
}
