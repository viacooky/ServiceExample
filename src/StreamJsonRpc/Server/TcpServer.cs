using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Server
{
    public class TcpServer
    {
        private string _host;
        private int    _port;
        private int    _clientId = 0;

        public TcpServer(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task StartAsync()
        {
            var ip       = IPAddress.Parse(_host);
            var listener = new TcpListener(ip, _port);
            listener.Start();
            Console.WriteLine($"开始监听{_port}");
            while (true)
            {
                var client         = await listener.AcceptTcpClientAsync();
                var remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"{remoteEndPoint?.Address} 已连接");
                var stream = client.GetStream();
                await ResponseAsync(stream, _clientId);
                _clientId++;
            }
        }

        private async Task ResponseAsync(NetworkStream stream, int clientId)
        {
            var jsonRpc = JsonRpc.Attach(stream, new HelloImpl());
            await jsonRpc.Completion;
            Console.WriteLine($"客户端 #{clientId} 的已断开连接");
            jsonRpc.Dispose();
            await stream.DisposeAsync();
        }
    }
}