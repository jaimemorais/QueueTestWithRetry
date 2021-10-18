using Polly;
using Polly.Retry;
using System.Collections.Concurrent;

namespace QueueWithRetry
{
    public class QueueWithRetryProcessor
    {
        private readonly ConcurrentQueue<object> _documentsQueue;

        private readonly AsyncRetryPolicy _retryPolicy;
        const int RETRY_ATTEMPTS = 3;


        public QueueWithRetryProcessor()
        {
            _documentsQueue = new ConcurrentQueue<object>();

            // retry policy using Polly
            _retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(RETRY_ATTEMPTS, attempt => {
                        var waitInSeconds = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                        Console.WriteLine($"  -- Polly [attempt {attempt}] waiting {waitInSeconds.TotalSeconds} seconds to try again ...");
                        return waitInSeconds;
                    }
                );


            Task queueProcessorTask = new(async () => await ProcessQueueAsync());
            queueProcessorTask.Start();    
        }


        public void RegisterDocument(object doc)
        {
            _documentsQueue.Enqueue(doc);
        }


        private async Task ProcessQueueAsync()
        {
            while (true)
            {
                if (!_documentsQueue.IsEmpty)
                {
                    if (_documentsQueue.TryDequeue(out object? doc))
                    {
                        await TryWrite(doc);
                    }
                }
            }
        }

        private async Task TryWrite(object doc)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    ThrowRandomExceptionForTestingPurpose(doc);

                    // write on backend here
                    await Task.Run(() => Console.WriteLine("Written : " + doc));
                });
            }
            catch 
            {
                Console.WriteLine($"  -- Exception not solved after {RETRY_ATTEMPTS} attempt(s).");
                // register problem here for prometheus alerting
            }
        }

        private static void ThrowRandomExceptionForTestingPurpose(object doc)
        {
            var teste = new Random().Next(0, 10);

            if (teste > 5)
            {
                Console.WriteLine($"  -- Throwing Exception for {doc}...");
                throw new Exception("Exception in TryWrite()");
            }
        }
    }
}
