using System.Collections.Generic;
using System.Linq;
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
        }

        public long LatestSequence { get; private set; }

        public async Task Initialize()
        {
            _logger?.LogInformation($"Initializing projection {GetType().Name}...");
            LatestSequence = await _repository.GetCurrentSequenceAsync();
            _logger?.LogInformation($"Initializing projection {GetType().Name} Done!");
        }

        public virtual async Task ApplyEventsAsync(IReadOnlyList<EventEnvelope> events)
        {
            if (!events.Any()) return;

            foreach (var @event in events)
            {
                await ((dynamic) this).ApplyEventAsync(@event.Sequence, (dynamic) @event.Body).ConfigureAwait(false);
                LatestSequence = @event.Sequence;
            }

            await _repository.SaveChangesAsync();
        }

        public async Task ClearAsync()
        {
            _logger?.LogInformation($"Clearing projection {GetType().Name}...");
            await _repository.DeleteAllAsync();
            LatestSequence = 0;
            _logger?.LogInformation($"Clearing projection {GetType().Name} Done!");
        }

        public virtual Task ApplyEventAsync(long sequence, object @event)
        {
            _logger?.LogInformation($"Event {@event.GetType().Name} ignored.");
            return Task.CompletedTask;
        }
    }
}