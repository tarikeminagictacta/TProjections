using System.Collections.Generic;
using TProjections.Core;

namespace TProjections.TestHost.Sample.Events
{
    public class EventStoreStream
    {
        private readonly string _bankId;

        public EventStoreStream(string bankId)
        {
            _bankId = bankId;
        }

        public IEnumerable<EventEnvelope> GetEnumerator()
        {
            yield return new EventEnvelope(new BankAccountCreated {Id = _bankId, AccountHolder = "Tarik Eminagic"}, 0);
            yield return new EventEnvelope(new MoneyDeposited {Id = _bankId, Amount = 2500}, 1);
            yield return new EventEnvelope(new MoneyWithdrawn {Id = _bankId, Amount = 100}, 2);
            yield return new EventEnvelope(new MoneyDeposited {Id = _bankId, Amount = 1500}, 3);
            yield return new EventEnvelope(new MoneyWithdrawn {Id = _bankId, Amount = 150}, 4);
            yield return new EventEnvelope(new MoneyWithdrawn {Id = _bankId, Amount = 1000}, 5);
            yield return new EventEnvelope(
                new BankAccountDetailsChanged {Id = _bankId, AccountHolder = "Haris Eminagic"}, 6);
        }
    }
}