using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Server
{
    public class NamedPipeServer
    {
        public NamedPipeServer(string pipeName)
        {
            _pipeName = pipeName;
        }

        private readonly string _pipeName;
        private          int    _clientId = 0;

        public async Task StartAsync()
        {
            while (true)
            {
                var stream = new NamedPipeServerStream(_pipeName,
                                                       PipeDirection.InOut,
                                                       3,
                                                       PipeTransmissionMode.Byte,
                                                       PipeOptions.Asynchronous);

                Console.WriteLine("等待连接");
                await stream.WaitForConnectionAsync();
                Console.WriteLine($"连接成功{_clientId}");
                _ = ResponseAsync(stream, _clientId);
                _clientId++;
            }
        }

        private async Task ResponseAsync(NamedPipeServerStream stream, int clientId)
        {
            var jsonRpc = JsonRpc.Attach(stream, new HelloImpl());
            await jsonRpc.Completion;
            Console.WriteLine($"客户端 #{clientId} 的已断开连接");
            jsonRpc.Dispose();
            await stream.DisposeAsync();
        }
    }
}