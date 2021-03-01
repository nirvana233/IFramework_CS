//using IFramework.Net;
//using IFramework.Net.WebSocket;
//using IFramework.Net;
//using System;

//namespace IFramework
//{
//    class program
//    {
//        private static void WebSocketDemo()
//        {
//            WSServerProvider wsService = new WSServerProvider();
//            wsService.OnAccepted = new OnAcceptedHandler((SocketToken sToken) => {
//                Console.WriteLine("accepted:" + sToken.TokenIpEndPoint);
//            });
//            wsService.OnDisconnected = new OnDisconnectedHandler((SocketToken sToken) => {
//                Console.WriteLine("disconnect:" + sToken.TokenIpEndPoint.ToString());
//            });
//            wsService.OnReceived = new OnReceivedHandler((SocketToken sToken, string content) => {

//                Console.WriteLine("receive:" + content);
//                wsService.Send(sToken, "hello websocket client! you said:" + content);

//            });
//            wsService.OnReceivedBytes = new OnReceivedSegmentHandler((SegmentToken session) => {
//                Console.WriteLine("receive bytes:" + session.Data.size);
//            });
//            bool isOk = wsService.Start(65531);
//            if (isOk)
//            {
//                Console.WriteLine("waiting for accept...");

//                WSClientProvider client = new WSClientProvider();
//                client.OnConnected = new OnConnectedHandler((SocketToken sToken, bool isConnected) => {
//                    Console.WriteLine("connected websocket server...");
//                });
//                client.OnReceived = new OnReceivedHandler((SocketToken sToken, string msg) => {
//                    Console.WriteLine(msg);
//                });

//                isOk = client.Connect("ws://127.0.0.1:65531");
//                if (isOk)
//                {
//                    client.Send("hello websocket");
//                }
//                Console.ReadKey();
//            }
//        }


//        static void Main(string[] args)
//        {
//            WebSocketDemo();


//            while (true)
//            {

//            }
//        }


//    }
//}
