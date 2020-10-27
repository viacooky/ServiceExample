using Contract;

namespace Server
{
    public class HelloImpl : IHello
    {
        public string Say(string name)
        {
            return $"Hello {name}";
        }
    }
}