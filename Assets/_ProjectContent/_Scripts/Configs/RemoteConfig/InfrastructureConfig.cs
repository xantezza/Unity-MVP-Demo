using System;
using Newtonsoft.Json;

namespace Configs.RemoteConfig
{
    [Serializable]
    public class InfrastructureConfig : IConfig
    {
        [JsonProperty] public float FakeTimeBeforeLoad = 0f;
        [JsonProperty] public float FakeMinimalLoadTime = 0.0f;
        [JsonProperty] public float FakeTimeAfterLoad = 0.0f;
        [JsonProperty] public int PlayerInventorySize = 6;
    }
}