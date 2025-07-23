namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    class ResourceName
    {
        protected ResourceName() { }

        protected ResourceName(string name)
        {
            Value = name;
        }

        string Value { get; } = "";

        public override string ToString() => Value;

        public bool Equals(ResourceName target) => target?.Value == Value;
    }
}
