namespace TProjections.Core
{
    public class EventEnvelope
    {
        public EventEnvelope(object body, long sequence)
        {
            Body = body;
            Sequence = sequence;
        }

        public object Body { get; }
        public long Sequence { get; }
    }
}