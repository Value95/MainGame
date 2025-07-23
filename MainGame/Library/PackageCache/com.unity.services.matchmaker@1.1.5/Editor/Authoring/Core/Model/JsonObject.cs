namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    internal class JsonObject
    {
        public JsonObject(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
