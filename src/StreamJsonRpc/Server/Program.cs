using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sharprompt;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("StreamJsonRpc Server 示例");
            var serverTypes = new Dictionary<int, string>
            {
                {0, "NamedPipe"},
                {1, "Tcp"}
            };
            var serverType = Prompt.Select("选择Server类型", serverTypes, null, serverTypes.AsEnumerable().First(), pair => pair.Value);
            switch (serverType.Key)
            {
                case 0:
                    var pipeName = Prompt.Input<string>("输入 PipeName", "Demo");
                    await new NamedPipeServer(pipeName).StartAsync();
                    break;
                case 1:
                    var host = Prompt.Input<string>("输入 host", "127.0.0.1");
                    var port = Prompt.Input<int>("输入 port", 10088);
                    await new TcpServer(host, port).StartAsync();
                    break;
                default:
                    Console.WriteLine("错误的选项");
                    break;
            }
        }
    }
}