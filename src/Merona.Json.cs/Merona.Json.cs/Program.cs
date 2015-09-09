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
