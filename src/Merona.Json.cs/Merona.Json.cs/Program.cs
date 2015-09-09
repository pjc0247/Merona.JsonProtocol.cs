using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Merona;
using Newtonsoft.Json;

namespace Merona.Json.cs
{
    public class JsonMarshaler : IMarshalContext
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
            try {
                var bytes = new byte[buffer.Size];
                buffer.Peek(bytes, 0, buffer.Size);

                var json = Encoding.UTF8.GetString(bytes);

                var stringReader = new StringReader(json);
                var reader = new JsonTextReader(stringReader);

                // 주의
                var field =
                    stringReader.GetType().GetField(
                        "_pos",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var depth = 0;
                var id = 0;
                var idField = true;
                var hasObject = false;

                try {
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
                                if (depth == 1 &&
                                    (String)reader.Value == "id")
                                    idField = true;
                                break;
                            case JsonToken.Integer:
                                if (idField)
                                {
                                    id = Convert.ToInt32(reader.Value);
                                    idField = false;
                                }
                                break;
                        }
                    }
                }
                catch(JsonReaderException e)
                {
                    // ignore
                }

              
                if (hasObject == false)
                    return null;
                if (depth != 0)
                    return null;

                var count = (int)field.GetValue(stringReader);
                buffer.Skip(count);

                var slicedJson = json.Substring(0, count);

                return (Packet)JsonConvert.DeserializeObject(slicedJson, Packet.GetTypeById(id));
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }

    [PacketId(1)]
    public class TestPacket : Packet
    {
        public String name;
    }

    class TestService : Service
    {

        [Handler(typeof(TestPacket))]
        public void OnTestPacket(Session session, TestPacket packet)
        {
            Console.WriteLine("OnTestPacket");

            session.Send(packet);
        }
    }

    class MySession : Session
    {
        public void OnConnected()
        {
            Console.WriteLine("OnConnected");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            NLog.Config.SimpleConfigurator.ConfigureForConsoleLogging(NLog.LogLevel.Debug);

            var config = Config.defaults;

            config.port = 9915;
            config.sessionType = typeof(MySession);
            config.marshalerType = typeof(JsonMarshaler);

            var server = new Server(config);
            server.AttachService<TestService>();
            server.Start();

            while (true)
            {
                //Console.WriteLine("running");
                Thread.Sleep(1000);
            }
        }
    }
}
