using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace DMaster
{
    public class Program
    {
        static void Main(string[] args)
        {
            var Bot = new bot();
            Bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
