using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TProjections.Core;

namespace TProjection.Pooling
{
    public class PoolingProjector<T> : IPoolingProjector where T : IProjection
    {
        private readonly IPassiveEventStore _eventStore;
        private readonly ILogger<T> _logger;
        private readonly T _projection;
        private CancellationTokenSource _cancellationTokenSource;

        public PoolingProjector(IPassiveEventStore eventStore, T projection, ILogger<T> logger)
        {
            _eventStore = eventStore;
            _projection = projection;
            _logger = logger;
            EventPageSize = 10;
            PoolingTimer = TimeSpan.FromSeconds(2);
        }

        protected int EventPageSize { get; }
        protected TimeSpan PoolingTimer { get; }

        public Task SetSequence()
        {
            return _projection.SetSequence();
        }

        public async Task StartProjecting()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var events = await _eventStore.GetEvents(_projection.LatestSequence, EventPageSize);
                    await _projection.ApplyEvents(events.ToList());

                    if (events.Count() == EventPageSize) continue;

                    await Task.Delay(PoolingTimer, _cancellationTokenSource.Token);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Projection {typeof(T).Name} stopped due error with exception message: {e.Message}");
                throw;
            }
        }

        public Task StopProjecting()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            return Task.CompletedTask;
        }
    }
}