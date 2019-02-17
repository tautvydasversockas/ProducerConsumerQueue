using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumerQueue
{
    public class ProducerConsumerQueue : IDisposable
    {
        private BlockingCollection<Task> _taskQueue = new BlockingCollection<Task>();

        public ProducerConsumerQueue(int workerCount)
        {
            for (int i = 0; i < workerCount; i++)
                Task.Run(() => Consume());
        }

        public Task Enqueue(Action action, CancellationToken cancellationToken = default)
        {
            var task = new Task(action, cancellationToken);
            _taskQueue.Add(task);
            return task;
        }

        public Task<TResult> Enqueue<TResult>(Func<TResult> func, CancellationToken cancellationToken = default)
        {
            var task = new Task<TResult>(func, cancellationToken);
            _taskQueue.Add(task);
            return task;
        }

        private void Consume()
        {
            foreach (var task in _taskQueue.GetConsumingEnumerable())
                try
                {
                    if (!task.IsCanceled)
                        task.RunSynchronously();
                }
                catch (InvalidOperationException)
                {
                    //Race condition
                }
        }

        public void Dispose()
        {
            _taskQueue.CompleteAdding();
        }
    }
}
