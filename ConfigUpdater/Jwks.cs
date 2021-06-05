using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ConfigUpdater
{
    public class Key
    {
        [JsonProperty("kty")]
        public string Kty { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        [JsonProperty("kid")]
        public string Kid { get; set; }

        [JsonProperty("use")]
        public string Use { get; set; }

        [JsonProperty("e")]
        public string E { get; set; }

        [JsonProperty("n")]
        public string N { get; set; }
    }

    public class Jwks
    {
        [JsonProperty("keys")]
        public IList<Key> Keys { get; set; }
    }
}
