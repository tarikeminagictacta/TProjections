using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TProjections.Core
{
    public abstract class Projection : IProjection
    {
        private readonly ILogger<Projection> _logger;
        private readonly IProjectionRepository _repository;

        protected Projection(IProjectionRepository repository,
            ILogger<Projection> logger = null)
        {
            _repository = repository;
            _logger = logger;
            LatestSequence = 0;
            EventPageSize = 50;
            PoolingTimer = TimeSpan.FromSeconds(3);
        }

        public TimeSpan PoolingTimer { get; }

        public int EventPageSize { get; }

        public long LatestSequence { get; private set; }

        public async Task SetSequence()
        {
            LatestSequence = await _repository.GetCurrentSequenceAsync();
            _logger.LogInformation($"[{GetType().Name}] Continuing projection from sequence: {LatestSequence}");
        }

        public async Task StartProjecting(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var events = await _repository.GetEvents(LatestSequence, EventPageSize);
                await ApplyEvents(events.OrderBy(e => e.Sequence).ToList());
                if (events.Count() < EventPageSize) await Task.Delay(PoolingTimer, cancellationToken);
            }
        }

        private async Task ApplyEvents(IReadOnlyList<EventEnvelope> events)
        {
            if (!events.Any()) return;

            foreach (var @event in events)
            {
                await ((dynamic) this).On(@event.Sequence, (dynamic) @event.Body).ConfigureAwait(false);
                LatestSequence = @event.Sequence;
            }

            await _repository.SaveChangesAsync();
        }

        public virtual Task On(long sequence, object @event)
        {
            _logger?.LogInformation($"Event {@event.GetType().Name} ignored.");
            return Task.CompletedTask;
        }
    }
}