using Grpc.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meeting.Core.GrpcClient.Tests.Models
{
    internal class AsyncStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IEnumerator<T> enumerator;

        public AsyncStreamReader(IEnumerable<T> results)
        {
            enumerator = results.GetEnumerator();
        }

        public T Current => enumerator.Current;

        public Task<bool> MoveNext(CancellationToken cancellationToken) =>
            Task.Run(() => enumerator.MoveNext());
    }
}
