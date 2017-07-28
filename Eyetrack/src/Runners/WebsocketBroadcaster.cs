using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tobii.Interaction;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Eyetrack.Runners
{
    public class WebsocketRunner : Runner
    {
        private static string WEBSOCKET_URL = "ws://localhost:8887";
        private static string WEBSOCKET_SERVICE_PATH = "/";

        private WebSocketServer server;
        private int gazeCount = 0;

        public override void start(GazeDispatcher gazeDispatcher)
        {
            gazeDispatcher.addEventHandler(onGazeEvent);
            this.server = new WebSocketServer(WEBSOCKET_URL);
            this.server.AddWebSocketService<GazeService>(WEBSOCKET_SERVICE_PATH);
            this.server.Start();
        }

        public override void stop()
        {
            this.server.Stop();
        }

        private void onGazeEvent(object sender, GazeEventArgs e)
        {
            var coordString = e.x + "/" + e.y;
            this.server.WebSocketServices[WEBSOCKET_SERVICE_PATH].Sessions.Broadcast(coordString);
        }

    }

    class GazeService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Message from client!");
        }
    }
}
