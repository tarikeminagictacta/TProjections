using System.Collections.Generic;
using System.Threading.Tasks;

namespace TProjections.Core
{
    public interface IProjection
    {
        long LatestSequence { get; }

        Task SetSequence();
        Task ApplyEvents(IReadOnlyList<EventEnvelope> events);
    }
}