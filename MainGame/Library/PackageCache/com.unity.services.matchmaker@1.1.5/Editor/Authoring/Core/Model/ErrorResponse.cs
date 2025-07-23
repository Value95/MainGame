using System.Runtime.Serialization;

namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    public class ErrorResponse
    {
        [DataMember(IsRequired = true)] public string ResultCode  {get; set;}

        [DataMember(IsRequired = true)] public string Message { get; set; }
    }
}
