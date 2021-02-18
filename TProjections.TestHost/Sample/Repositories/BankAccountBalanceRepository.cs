using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TProjections.Core;
using TProjections.TestHost.Sample.Events;

namespace TProjections.TestHost.Sample.Repositories
{
    public class BankAccountBalanceRepository : IBankAccountBalanceRepository
    {
        private readonly List<BankAccountBalance> _accountBalances;

        public BankAccountBalanceRepository()
        {
            _accountBalances = new List<BankAccountBalance>();
        }

        public IReadOnlyList<BankAccountBalance> AccountBalances => _accountBalances.AsReadOnly();

        public async Task<long> GetCurrentSequenceAsync()
        {
            return 1;
        }

        public async Task<ICollection<EventEnvelope>> GetEvents(long latestSequence, int eventPageSize)
        {
            var events = new List<EventEnvelope>();
            var random = new Random(1337);
            if (latestSequence == 1)
                events.Add(new EventEnvelope(
                    new BankAccountCreated {AccountHolder = "Tarik Eminagic", Id = "some-account"}, 1));
            while (events.Count < eventPageSize - 10)
                if (events.Count % 2 == 0)
                    events.Add(new EventEnvelope(
                        new MoneyDeposited {Amount = Convert.ToDecimal(random.Next(0, 1337)), Id = "some-account"},
                        events.Count + 1));
                else
                    events.Add(new EventEnvelope(
                        new MoneyWithdrawn {Amount = Convert.ToDecimal(random.Next(0, 1337)), Id = "some-account"},
                        events.Count + 1));

            return events;
        }

        public async Task SaveChangesAsync()
        {
            // Do nothing
        }

        public async Task RegisterAsync(string eventId, long sequence)
        {
            _accountBalances.Add(new BankAccountBalance {Balance = 0M, Id = eventId, Sequence = sequence});
        }

        public async Task<BankAccountBalance> GetAsync(string eventId)
        {
            return _accountBalances.First(b => b.Id == eventId);
        }

        public async Task UpdateBalance(BankAccountBalance balance, long sequence)
        {
            var existingBalance = await GetAsync(balance.Id);
            _accountBalances.Remove(existingBalance);
            _accountBalances.Add(balance);
        }
    }
}