using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TProjections.Core;

namespace TProjections.TestHost
{
    public class Worker : BackgroundService
    {
        private readonly IEnumerable<IProjection> _projections;
        private readonly List<Task> _subscriptions;

        public Worker(IEnumerable<IProjection> projections)
        {
            _projections = projections;
            _subscriptions = new List<Task>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var projection in _projections)
            {
                await projection.SetSequence();
                _subscriptions.Add(Task.Run(() => projection.StartProjecting(stoppingToken), stoppingToken));
            }
        }
    }
}