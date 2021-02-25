using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using IFramework;
using IFramework.Net;
using IFramework.Net.Http;
using IFramework.Packets;

namespace Example
{
    public class NetTest: Test
    {
        protected override void Start()
        {
            TcpDemo();
            // UdpDemo();
            // ProtocolsDemo();
            // ConnectionPoolManagerDemo();
            //   ConnectionPoolTest();
            //ConnectDemo();
            //PacketSocketDemo();
            //PacketDemo();
           // WebSocketDemo();
           // HttpTest();
        }

        private static void TcpDemo()
        {
            int port = 12145;

            //服务端
            var serverSocket = NetTool.CreateTcpSever(maxNumberOfConnections: 2);

            serverSocket.ReceivedOffsetCallback = new OnReceivedSegmentHandler((SegmentToken session) =>
            {
                try
                {
                    //feedback message to client
                    serverSocket.Send(new SegmentToken(session.sToken, Encoding.Default.GetBytes("welcome   " + DateTime.Now)));

                    Log.L("from client   " + Encoding.Default.GetString(session.Data.buffer,
                        session.Data.offset, session.Data.size) + Environment.NewLine);

                    //string info = Encoding.UTF8.GetString(buff, offset, count);
                    // Log.L(count);
                }
                catch (Exception ex)
                {
                    Log.L(ex.ToString());
                }
            });
            serverSocket.AcceptedCallback = new OnAcceptedHandler((sToken) =>
            {
                Log.L("accept" + sToken.TokenIpEndPoint + "\n");
            });

            serverSocket.DisconnectedCallback = new OnDisconnectedHandler((stoken) =>
            {
                Log.L(" server show disconnect" + stoken.TokenId);
            });

            bool isOk = serverSocket.Start(port);
            if (isOk)
            {
                Log.L("已启动服务。。。");

                //客户端
                var clientSocket = NetTool.CreateTcpClient();

                //同步发送接收
                //isOk = clientSocket.ConnectTo(port, "127.0.0.1");
                //if (isOk)
                //{
                //SegmentOffset receive = new SegmentOffset(new byte[4096]);
                //clientSocket.SendSync(new SegmentOffset(Encoding.Default.GetBytes("hello")), receive);
                //Log.L(Encoding.Default.GetString(receive.buffer));

                //同步接收数据
                //clientSocket.SendSync(new SegmentOffset(Encoding.Default.GetBytes("hello")), null);

                //var t = Task.Run(() =>
                //{
                //    clientSocket.ReceiveSync(receive, (data) =>
                //    {
                //        Log.L(Encoding.Default.GetString(receive.buffer));
                //    });
                //});

                //clientSocket.SendSync(new SegmentOffset(Encoding.Default.GetBytes("hello1111")), null);
                //}
                //return;

                //异步连接
                clientSocket.ReceivedOffsetCallback = new OnReceivedSegmentHandler((SegmentToken session) =>
                {
                    try
                    {
                        Log.L("from server" + Encoding.Default.GetString(session.Data.buffer,
                            session.Data.offset, session.Data.size) + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {

                    }
                });
                clientSocket.DisconnectedCallback = new OnDisconnectedHandler((stoken) =>
                {
                    Log.L("clinet show discount");
                });
            again:
                bool rt = clientSocket.ConnectTo(port, "127.0.0.1");/* 10.152.0.71*/
                if (rt)
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        if (i % 1000 == 0)
                        {
                            Log.L(clientSocket.SendBufferPoolNumber + ":" + i);
                        }
                        clientSocket.Send(new SegmentOffset(Encoding.Default.GetBytes("client send    " + DateTime.Now)), false);
                        //break;
                    }
                    //byte[] buffer = System.IO.File.ReadAllBytes("TRANSACTION_EXTRANSACTIONUPLOAD_REQ_52_1000_20171031143825836.json");

                    //clientSocket.Send(buffer);

                    //Log.L("complete:sent:" + sentlength.ToString() + "rec:" + reclength.ToString());
                    int ab = 0;
                    while (true)
                    {
                        Thread.Sleep(3000);
                        Log.L("retry :pool:" + clientSocket.SendBufferPoolNumber);
                        if (ab++ >= 1) break;
                    }

                    //var c = Console.ReadKey();
                    //if (c.KeyChar == 'r') goto again;
                    serverSocket.Stop();

                    //clientSocket.Disconnect();
                }
            }
            Console.ReadKey();
            serverSocket.Dispose();
        }

        private static void UdpDemo()
        {
            int port = 12345;
            int svc_c = 0, cli_c = 0, cli_c2 = 0;
            var serverProvider = NetTool.CreateUdpSever(4096, 32);
            serverProvider.ReceivedOffsetHanlder = new OnReceivedSegmentHandler((SegmentToken session) =>
            {
                ++svc_c;

                Log.L("from client:" + Encoding.UTF8.GetString(session.Data.buffer, session.Data.offset, session.Data.size));
                serverProvider.Send(new SegmentOffset(Encoding.UTF8.GetBytes("i'm server" + DateTime.Now)), session.sToken.TokenIpEndPoint);
            });
            serverProvider.Start(port);
            var clientProvider = NetTool.CreateUdpClient(4096, 4);
            clientProvider.ReceivedOffsetHandler = new OnReceivedSegmentHandler((SegmentToken session) =>
            {
                Log.L("from server :" + Encoding.UTF8.GetString(session.Data.buffer, session.Data.offset,
                    session.Data.size));

                ++cli_c;
            });
            bool isConn = clientProvider.Connect(port, "127.0.0.1");

            int c = 10;

            while (c > 0)
            {

                //string msg = Console.ReadLine();
                //if (msg == "exit")
                //    break;

                clientProvider.Send(new SegmentOffset(Encoding.UTF8.GetBytes((--c).ToString())));

            }
            Log.L(string.Format("完成svc: {0} ;cli1: {1} ;cli2: {2}", svc_c, cli_c, cli_c2));

            Console.ReadKey();
            serverProvider.Dispose();
            clientProvider.Dispose();
        }

        private static void ProtocolsDemo()
        {
            INetProtocolProvider protocolProvider = NetTool.CreateProtocolProvider();

            //数据内容打包成字节
            byte[] content = new byte[] { 1, 3, 4, 0xfe, 0x01, 0xfd, 0x02 };
            byte[] buffer = protocolProvider.Encode(new Packet(1, 0x10, 5, content));

            //使用接收管理缓冲池解析数据包
            INetPacketProvider pkgProvider = NetTool.CreatePacketsProvider(1024);
            bool rt = pkgProvider.SetBlocks(buffer, 0, buffer.Length);
            rt = pkgProvider.SetBlocks(buffer, 0, buffer.Length);
            var dePkg = pkgProvider.GetBlocks();

            //解析数据包成结构信息
            // var dePkg = protocolProvider.Decode(buffer, 0, buffer.Length);
        }

        private static void ConnectionPoolManagerDemo()
        {
            int port = 13145;

            var netServerProvider = NetTool.CreateTcpSever();
            INetTokenPoolProvider tokenPool = NetTool.CreateTokenPoolProvider(60);
            tokenPool.ConnectionTimeout = 60;
            SocketToken _sToken = null;

            netServerProvider.AcceptedCallback = new OnAcceptedHandler((sToken) => {
                _sToken = sToken;
                tokenPool.InsertToken(new NetConnectionToken()
                {
                    Token = sToken
                });
            });

            bool isOk = netServerProvider.Start(port);
            if (isOk)
            {
                var netClientProvider = NetTool.CreateTcpClient();
                netClientProvider.DisconnectedCallback = new OnDisconnectedHandler((sToken) =>
                {
                    Log.L("client disconnected");
                });
                bool rt = netClientProvider.ConnectTo(port, "127.0.0.1");
                if (rt)
                {
                    while (tokenPool.Count == 0)
                    {
                        Thread.Sleep(10);
                    }
                    var rtToken = tokenPool.GetTokenBySocketToken(_sToken);
                    bool refreshRt = tokenPool.RefreshExpireToken(_sToken);
                    Log.L("pool count:" + tokenPool.Count);
                    Console.ReadKey();
                }
            }

        }

        private static void ConnectionPoolTest()
        {
            var serverProvider = NetTool.CreateTcpSever(4096, 2);
            INetTokenPoolProvider poolProvider = NetTool.CreateTokenPoolProvider(60);
            List<IFramework.Net.Tcp.ITcpClientProvider> clientPool = new List<IFramework.Net.Tcp.ITcpClientProvider>();

            poolProvider.TimerEnable(false);

            int port = 12345;

            serverProvider.DisconnectedCallback = new OnDisconnectedHandler((s) =>
            {
                Log.L("server disconnected:" + s.TokenId);
            });
            serverProvider.AcceptedCallback = new OnAcceptedHandler((s) =>
            {
                Log.L("accept:" + s.TokenId);
                poolProvider.InsertToken(new NetConnectionToken(s));
            });
            serverProvider.ReceivedOffsetCallback = new OnReceivedSegmentHandler((token) => {
                Log.L("server receive" + token.sToken.TokenId + ":" + Encoding.Default.GetString(token.Data.buffer, token.Data.offset, token.Data.size));
            });
            bool isStart = serverProvider.Start(port);
            if (isStart)
            {
            again:
                for (int i = 0; i < 3; ++i)
                {
                    var clientProvider = NetTool.CreateTcpClient();
                    clientProvider.DisconnectedCallback = new OnDisconnectedHandler((s) =>
                    {
                        Log.L(" client disconnected:" + s.TokenId);
                    });
                    //clientProvider.ReceiveOffsetHandler = new OnReceiveOffsetHandler((SegmentToken session) =>
                    //{
                    //    Log.L(session.sToken.TokenIpEndPoint + Encoding.Default.GetString(session.Data.buffer,
                    //        session.Data.offset, session.Data.size));
                    //});
                    bool isConnected = clientProvider.ConnectTo(port, "127.0.0.1");
                    if (isConnected) clientPool.Add(clientProvider);

                    Log.L("connect:" + isConnected);
                }
            send:
                Log.L(poolProvider.Count);
                string info = Console.ReadLine();

                if (info == "send")
                {
                    for (int i = 0; i < poolProvider.Count; ++i)
                    {
                        var item = poolProvider.GetTokenById(i);
                        if (item == null) continue;

                        serverProvider.Send(new SegmentToken(item.Token, Encoding.Default.GetBytes(DateTime.Now.ToString())));
                        Thread.Sleep(1000);
                        // poolProvider.Clear(true);
                        //var item = poolProvider.GetTopToken();
                        //if (item != null)
                        //{
                        //    serverProvider.CloseToken(item.Token);
                        //    poolProvider.RemoveToken(item, false);
                        //}
                    }
                    goto send;
                }
                else if (info == "stop")
                {
                    serverProvider.Stop();
                    goto again;
                }
                else if (info == "clear")
                {
                    poolProvider.Clear();
                    clientPool.Clear();

                    goto again;
                }
                else if (info == "client")
                {
                    for (int i = 0; i < clientPool.Count; ++i)
                    {
                        clientPool[i].Send(new SegmentOffset(Encoding.Default.GetBytes(DateTime.Now.ToString())));
                        Thread.Sleep(200);
                    }
                    goto send;
                }
                Console.ReadKey();
            }
        }

        private static void ConnectDemo()
        {
            try
            {
                SocketToken sToken = null;
                var serverProvider = NetTool.CreateTcpSever(4096, 2);
                serverProvider.DisconnectedCallback = new OnDisconnectedHandler((SocketToken stoken) => {
                    Log.L("client disconnected" + stoken.TokenIpEndPoint);
                });
                serverProvider.AcceptedCallback = new OnAcceptedHandler((token) => {
                    Log.L("accpet" + token.TokenIpEndPoint);
                    sToken = token;
                });

                bool isOk = serverProvider.Start(12345);
                if (isOk)
                {
                    var clientProvider = NetTool.CreateTcpClient();
                    clientProvider.ConnectedCallback = new OnConnectedHandler((SocketToken stoken, bool isConnected) =>
                    {
                        Log.L("connected" + stoken.TokenIpEndPoint);
                    });
                    clientProvider.DisconnectedCallback = new OnDisconnectedHandler((SocketToken stoken) =>
                    {
                        Log.L("disconnected" + stoken.TokenIpEndPoint);
                    });
                again:
                    isOk = clientProvider.ConnectTo(12345, "127.0.0.1");
                    Log.L(isOk);
                    string info = Console.ReadLine();
                    if (info == "again")
                    {
                        //clientProvider.Disconnect();
                        serverProvider.Close(sToken);
                        goto again;
                    }
                    Log.L("exit");
                }
            }
            catch (Exception ex)
            {
                Log.L(ex.Message);
            }
            Console.Read();
        }

        private static void PacketSocketDemo()
        {
            int port = 13145;
            var netServerProvider = NetTool.CreateTcpSever();
            INetProtocolProvider netProtocolProvider = NetTool.CreateProtocolProvider();
            INetPacketProvider netPacketProvider = NetTool.CreatePacketsProvider(4096 * 32);//最大容量,必须大于发送缓冲区，建议是设置大于8倍以上

            int pktCnt = 0;
            netServerProvider.ReceivedOffsetCallback = new OnReceivedSegmentHandler((SegmentToken session) =>
            {
                bool isEn = netPacketProvider.SetBlocks(session.Data.buffer, session.Data.offset, session.Data.size);
                if (isEn == false)
                {
                    Log.L("entry queue failed");
                }
                List<Packet> packets = netPacketProvider.GetBlocks();

                pktCnt += packets.Count;

                if (packets.Count > 0)
                {
                    foreach (var pkt in packets)
                    {
                        Log.L(Encoding.UTF8.GetString(pkt.message));
                    }
                }
                else
                {
                    Log.L("got null item from queue");
                }

                Log.L("pktCnt:" + pktCnt);
            });

            bool isStart = netServerProvider.Start(port);
            if (isStart)
            {
                var netClientProvider = NetTool.CreateTcpClient();
                bool isConneted = netClientProvider.ConnectTo(port, "127.0.0.1");
                if (isConneted)
                {
                    //for (int i = 0; i < content.Length; ++i)
                    //{
                    //    content[i] = (byte)(i > 255 ? 255 : i);
                    //}
                    int i = 0;
                    for (; i < 100000; ++i)
                    {
                        byte[] content = Encoding.UTF8.GetBytes("hello 哈哈 http://anystore.bouyeijiang.com" + DateTime.Now + i.ToString());

                        byte[] buffer = netProtocolProvider.Encode(new Packet(1, 0x10, 5, content));

                        netClientProvider.Send(new SegmentOffset(buffer));
                    }
                }

                Console.ReadKey();
            }

        }

        private static void PacketDemo()
        {
            var protocolProvider = NetTool.CreateProtocolProvider();
            var packetProvider = NetTool.CreatePacketsProvider(4096 * 32);

            var pk1 = protocolProvider.Encode(new Packet(1, 0x10, 5, new byte[] { 1, 2 }));
            var pk2 = protocolProvider.Encode(new Packet(1, 0x10, 5, new byte[] { 3, 4 }));


            List<byte> buffer = new List<byte>();
            buffer.AddRange(pk1);
            //混淆测试值
            buffer.Add(11);
            buffer.AddRange(pk2);
            //混淆测试值
            buffer.Add(6);

            bool isOk = packetProvider.SetBlocks(buffer.ToArray(), 0, buffer.Count);
            var pks = packetProvider.GetBlocks();
        }

        private static void WebSocketDemo()
        {
            var wsService = NetTool.CreateWSSever();
            wsService.OnAccepted = new OnAcceptedHandler((SocketToken sToken) => {
                Log.L("accepted:" + sToken.TokenIpEndPoint);
            });
            wsService.OnDisconnected = new OnDisconnectedHandler((SocketToken sToken) => {
                Log.L("disconnect:" + sToken.TokenIpEndPoint.ToString());
            });
            wsService.OnReceived = new OnReceivedHandler((SocketToken sToken, string content) => {

                Log.L("receive:" + content);
                wsService.Send(sToken, "hello websocket client! you said:" + content);

            });
            wsService.OnReceivedBytes = new OnReceivedSegmentHandler((SegmentToken session) => {
                Log.L("receive bytes:" + session.Data.size);
            });
            bool isOk = wsService.Start(2222);
            if (isOk)
            {
                Log.L("waiting for accept...");

                var client = NetTool.CreateWSClient();
                client.OnConnected = new OnConnectedHandler((SocketToken sToken, bool isConnected) => {
                    Log.L("connected websocket server...");
                });
                client.OnReceived = new OnReceivedHandler((SocketToken sToken, string msg) => {
                    Log.L(msg);
                });

                isOk = client.Connect("ws://127.0.0.1:2222");
                if (isOk)
                {
                    client.Send("hello websocket");
                }
            }
        }

        private static void HttpTest()
        {
            var httpServer = NetTool.CreateHttpSever();
            httpServer.hOnReceived = new HttpOnReceived((payload) =>
            {
                Log.L(payload.Header.Option);
                Log.L(payload.Header.RelativeUri);

                if (payload.Header.Option == HttpOption.GET)
                {
                    HttpGet get = new HttpGet(payload.Header);
                    var rsp = get.Response(DateTime.Now.ToString());

                    httpServer.Send(payload.Token, Encoding.UTF8.GetBytes(rsp));
                    httpServer.Disconnect(payload.Token);
                }

            });
            bool isOk = httpServer.Start();
            if (isOk)
            {
                Log.L("listening on:" + httpServer.Port);
            }
        }

       

        protected override void Update()
        {
           
        }

        protected override void Stop()
        {
            
        }
    }

}
