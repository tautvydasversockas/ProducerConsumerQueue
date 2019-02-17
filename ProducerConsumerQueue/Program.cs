using System;
using System.Threading.Tasks;

namespace ProducerConsumerQueue
{
    class Program
    {
        static async Task Main()
        {
            var producerConsumerQueue = new ProducerConsumerQueue(workerCount: 2);
            var result = await producerConsumerQueue.Enqueue(() => "Hello, world!");
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
