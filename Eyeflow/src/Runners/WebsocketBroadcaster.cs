using System;
using WebSocketSharp;
using WebSocketSharp.Server;

using Eyeflow.Dispatchers;

namespace Eyeflow.Runners
{
    public class WebsocketBroadcaster : Runner
    {
        private static string WEBSOCKET_URL = "ws://localhost:8887";
        private static string WEBSOCKET_SERVICE_PATH = "/";

        private WebSocketServer server;

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
