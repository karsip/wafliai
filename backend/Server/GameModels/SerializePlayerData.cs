using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GameModels
{
    public class SerializePlayerData : JsonConverter<IPAddress>
    {
        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            return IPAddress.Parse(s);
        }

        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
