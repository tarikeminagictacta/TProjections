using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TProjections.Core;
using TProjections.TestHost.Sample.Events;

namespace TProjections.TestHost
{
    public class Worker : BackgroundService
    {
        private readonly IEnumerable<IProjection> _projections;

        public Worker(IEnumerable<IProjection> projections)
        {
            _projections = projections;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var events = new EventStoreStream("SomeBankAccount").GetEnumerator().ToList();
            foreach (var projection in _projections)
            {
                await projection.Initialize();
                await projection.ApplyEventsAsync(events);
            }
        }
    }
}