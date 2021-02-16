using System.Collections.Generic;
using System.Threading.Tasks;

namespace TProjections.Core
{
    public interface IProjection
    {
        long LatestSequence { get; }
        Task Initialize();
        Task ApplyEventsAsync(IReadOnlyList<EventEnvelope> events);
        Task ClearAsync();
    }
}