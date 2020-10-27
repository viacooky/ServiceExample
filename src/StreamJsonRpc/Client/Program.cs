using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contract;
using Sharprompt;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("StreamJsonRpc Client 示例");
            var clientTypes = new Dictionary<int, string>
            {
                {0, "NamedPipe"},
                {1, "Tcp"},
            };
            var clientType = Prompt.Select("选择 Client 类型", clientTypes, null, clientTypes.AsEnumerable().First(), pair => pair.Value);
            switch (clientType.Key)
            {
                case 0:
                {
                    var serverName = Prompt.Input<string>("输入 ServerName", ".");
                    var pipeName   = Prompt.Input<string>("输入 PipeName", "Demo");

                    while (true)
                    {
                        var targetName = Prompt.Input<string>("输入 targetName", nameof(IHello.Say));
                        var input      = Prompt.Input<string>("输入参数") ?? string.Empty;
                        await NamedPipeClient.SendRequestAsync(serverName, pipeName, targetName, input);
                    }
                }
                case 1:
                {
                    var host   = Prompt.Input<string>("输入 host", "127.0.0.1");
                    var port   = Prompt.Input<int>("输入 port", 10088);
                    var client = new TcpClientDemo(host, port);
                    while (true)
                    {
                        var targetName = Prompt.Input<string>("输入 targetName", nameof(IHello.Say));
                        var input      = Prompt.Input<string>("输入参数") ?? string.Empty;
                        await client.InvokeAsync<string>(targetName, input);
                    }
                }
                default:
                    Console.WriteLine("错误的选项");
                    break;
            }
        }
    }
}