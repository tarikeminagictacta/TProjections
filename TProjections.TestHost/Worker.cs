using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TProjection.Pooling;

namespace TProjections.TestHost
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Task> _subscriptions;

        public Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
            _subscriptions = new List<Task>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var projectors = scope.ServiceProvider.GetServices<IPoolingProjector>().ToList();

            try
            {
                foreach (var projector in projectors)
                {
                    await projector.SetSequence();
                    _subscriptions.Add(Task.Run(() => projector.StartProjecting(), stoppingToken));
                }

                while (!stoppingToken.IsCancellationRequested)
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            finally
            {
                foreach (var projector in projectors) await projector.StopProjecting();
                foreach (var subscription in _subscriptions) subscription.Dispose();
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}