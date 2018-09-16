using System;
using System.Threading.Tasks;

namespace ServiceBusSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            await new BusQueueUsingPlugins()
                .Invoke();
        }
    }
}