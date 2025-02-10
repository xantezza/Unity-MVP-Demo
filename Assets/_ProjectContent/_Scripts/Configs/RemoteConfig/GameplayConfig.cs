using System;
using Newtonsoft.Json;

namespace Configs.RemoteConfig
{
    [Serializable]
    public class GameplayConfig : IConfig
    {
        [JsonProperty] public int PlayerInventorySize = 6;
    }
}