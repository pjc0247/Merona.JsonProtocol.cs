using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Merona;
using Newtonsoft.Json;

namespace Merona.Json.cs
{
    public class JsonMarshaler : Server.IMarshalContext
    {
        public byte[] Serialize(CircularBuffer<Packet> buffer)
        {
            if (buffer.Size == 0)
                return null;
            
            var json = JsonConvert.SerializeObject(buffer.Get());
            return Encoding.UTF8.GetBytes(json);
        }
        public Packet Deserialize(CircularBuffer<byte> buffer)
        {
            var bytes = new byte[buffer.Size];
            buffer.Peek(bytes, 0, buffer.Size);

            var json = Encoding.UTF8.GetString(bytes);

            var stringReader = new StringReader(json);
            var reader = new JsonTextReader(stringReader);

            // 주의
            var field =
                reader.GetType().GetField(
                    "_pos",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var depth = 0;
            var id = 0;
            var idField = true;
            var hasObject = false;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        depth++;
                        hasObject = true;
                        break;
                    case JsonToken.EndObject:
                        depth--;

                        if (depth == 0)
                            break;
                        break;
                        
                    case JsonToken.PropertyName:
                        if (depth == 1)
                            idField = true;
                        break;
                    case JsonToken.Integer:
                        if (idField)
                        {
                            id = (int)reader.Value;
                            idField = false;
                        }
                        break;
                }


            }

            if (hasObject == false)
                return null;

            var count = (int)field.GetValue(stringReader);
            buffer.Skip(count);

            var slicedJson = json.Substring(0, count);

            JsonConvert.DeserializeObject(slicedJson, Packet.GetTypeById(id));

            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
