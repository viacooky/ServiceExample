using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Client
{
    public class NamedPipeClient
    {
        private readonly string                _serverName;
        private readonly string                _pipeName;
        private          NamedPipeClientStream _stream;
        private const    int                   Timeout = 1_000; // ms

        public NamedPipeClient(string serverName, string pipeName)
        {
            _serverName = serverName;
            _pipeName   = pipeName;
        }

        /// <summary>
        ///     连接
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            _stream = new NamedPipeClientStream(_serverName,
                                                _pipeName,
                                                PipeDirection.InOut,
                                                PipeOptions.Asynchronous);
            Console.WriteLine("连接开始");
            await _stream.ConnectAsync(Timeout);
            Console.WriteLine("连接完成");
        }

        /// <summary>
        ///     调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> InvokeAsync<T>(string targetName, params object[] args)
        {
            await ConnectAsync();
            var jsonRpc = JsonRpc.Attach(_stream);
            var rs      = await jsonRpc.InvokeAsync<T>(targetName, args);
            await _stream.DisposeAsync();
            return rs;
        }

        /// <summary>
        ///     发送请求
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <param name="targetName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task SendRequestAsync(string serverName, string pipeName, string targetName, params string[] args)
        {
            try
            {
                var client = new NamedPipeClient(serverName, pipeName);
                var rs     = await client.InvokeAsync<string>(targetName, args);
                Console.WriteLine(rs);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
            }
        }
    }
}