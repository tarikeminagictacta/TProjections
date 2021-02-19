using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TProjections.Core
{
    public abstract class Projection : IProjection
    {
        private readonly IProjectionRepository _repository;

        protected Projection(IProjectionRepository repository)
        {
            _repository = repository;
            LatestSequence = 0;
        }

        public long LatestSequence { get; private set; }

        public async Task SetSequence()
        {
            LatestSequence = await _repository.GetCurrentSequenceAsync();
        }

        public async Task ApplyEvents(IReadOnlyList<EventEnvelope> events)
        {
            if (!events.Any()) return;

            foreach (var @event in events.OrderBy(e => e.Sequence))
            {
                await ((dynamic) this).On((dynamic) @event.Body, @event.Sequence).ConfigureAwait(false);
                LatestSequence = @event.Sequence;
            }

            await _repository.SaveChangesAsync();
        }

        public virtual Task On(object @event, long sequence)
        {
            return Task.CompletedTask;
        }
    }
}