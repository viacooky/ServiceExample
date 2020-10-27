using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Client
{
    public class TcpClientDemo
    {
        private readonly string    _host;
        private readonly int       _port;
        private          TcpClient _client;

        public TcpClientDemo(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task InvokeAsync<T>(string targetName, params object[] args)
        {
            _client = new TcpClient();
            try
            {
                Console.WriteLine("连接开始");
                if (!_client.Connected) await _client.ConnectAsync(_host, _port);
                Console.WriteLine("连接完成");
                var stream  = _client.GetStream();
                var jsonRpc = JsonRpc.Attach(stream);
                var rs      = await jsonRpc.InvokeAsync<T>(targetName, args);
                Console.WriteLine(rs);
                await stream.DisposeAsync();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
            }

            _client.Close();
        }
    }
}