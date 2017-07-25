using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tobii.Interaction;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Eyetrack.Runners.WebsocketExample
{
    class GazeService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Message!!");
        }
    }

    public class WebsocketRunner : Runner
    {
        static WebSocketServer server;
        static int gazeCount = 0;

        static void onGaze(double x, double y, double timestamp)
        {
            gazeCount++;
            if (gazeCount % 50 == 0) {
                var coordString = x + "/" + y;
                Console.WriteLine("Broadcast: " + coordString);
                server.WebSocketServices["/"].Sessions.Broadcast(coordString);
            }
        }

        public override void run()
        {
            server = new WebSocketServer("ws://localhost:8887");
            server.AddWebSocketService<GazeService>("/");
            server.Start();

            var host = new Host();
            var gazePointDataStream = host.Streams.CreateGazePointDataStream();
            gazePointDataStream.GazePoint(onGaze);
            Console.ReadKey();
            host.DisableConnection();
            server.Stop();
        }
    }
}
