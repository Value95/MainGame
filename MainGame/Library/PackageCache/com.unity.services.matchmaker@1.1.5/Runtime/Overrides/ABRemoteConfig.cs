using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Http;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Services.Matchmaker.Models;
using Debug = Unity.Services.Matchmaker.Logger;

namespace Unity.Services.Matchmaker.Overrides
{
    internal class ABRemoteConfig : IABRemoteConfig
    {
        private const string StagingEnvironment = "staging";
        private const string ConfigType = "matchmaker";
        private const string StagingURL = "https://remote-config-stg.uca.cloud.unity3d.com/api/v1/settings";
        private const string ProdURL = "https://config.unity3d.com/api/v1/settings";

        private readonly IHttpClient _client;
        private readonly string _userId;
        private readonly string _url;
        private bool _needRefresh = true;
        private readonly string _projectId;
        private readonly string _environmentId;

        public List<Override> Overrides { get; private set; }

        public string AssignmentId { get; private set; }

        public ABRemoteConfig(IHttpClient client, string installationId, string cloudEnvironment, string projectId, string environmentId)
        {
            Overrides = new List<Override>();

            _client = client;
            _userId = installationId;
            _projectId = projectId;
            _environmentId = environmentId;
            _url = cloudEnvironment == StagingEnvironment ? StagingURL : ProdURL;
        }

        public async Task RefreshGameOverridesAsync()
        {
            if (!_needRefresh)
            {
                return;
            }

            var requestData = new
            {
                projectId = _projectId,
                environmentId = _environmentId,
                userId = _userId,
                configType = ConfigType,
                attributes = new Dictionary<string, Dictionary<string, string>> {
                    ["unity"] = new Dictionary<string, string>
                    {
                        { "platform", Application.platform.ToString() }
                    },

                    ["app"] = new Dictionary<string, string>(),
                    ["user"] = new Dictionary<string, string>(),
                }
            };

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestData));
            var headers = new Dictionary<string, string>();

            var response = await _client.MakeRequestAsync("POST", _url, body, headers, 10);
            if (response.IsHttpError || response.IsNetworkError)
            {
                Debug.LogError($"Error while fetching overrides: code={response.StatusCode}, message='{response.ErrorMessage}'");
                return;
            }

            var responseBodyStr = Encoding.UTF8.GetString(response.Data);
            var responseBody = JObject.Parse(responseBodyStr);
            if (responseBody["configs"]?[ConfigType]?.Type != JTokenType.Object)
            {
                Debug.LogError($"Error while fetching overrides: '{ConfigType}' missing in {responseBodyStr}");
                return;
            }

            if (responseBody["metadata"]?["assignmentId"]?.Type != JTokenType.String)
            {
                Debug.LogError($"Error while fetching overrides: 'assignmentId' missing in {responseBodyStr}");
                return;
            }
            AssignmentId = responseBody["metadata"]?["assignmentId"].Value<string>();

            foreach (var jToken in responseBody["configs"][ConfigType])
            {
                var overrideInfo = (JProperty)jToken;
                if (overrideInfo == null) continue;
                var variantId = overrideInfo.Value["variantId"];
                if (variantId == null) continue;
                var overrideId = overrideInfo.Value["overrideId"];
                if (overrideId == null) continue;

                Overrides.Add(new Override(
                    overrideInfo.Name,
                    variantId.Value<string>(),
                    overrideId.Value<string>()));
            }

            _needRefresh = false;
        }

    }
}
