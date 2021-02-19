using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TProjection.Pooling;
using TProjections.Core;
using TProjections.TestHost.Sample.Events;

namespace TProjections.TestHost.Sample.Repositories
{
    public class BankAccountBalanceRepository : IBankAccountBalanceRepository, IPassiveEventStore
    {
        private readonly List<BankAccountBalance> _accountBalances;
        private readonly Random _random;

        public BankAccountBalanceRepository()
        {
            _accountBalances = new List<BankAccountBalance>();
            _random = new Random();
            BankAccount = _random.Next(1, 1000);
        }

        public IReadOnlyList<BankAccountBalance> AccountBalances => _accountBalances.AsReadOnly();

        public Task<long> GetCurrentSequenceAsync()
        {
            return Task.FromResult<long>(0);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public int BankAccount { get; }

        public Task RegisterAsync(string eventId, long sequence)
        {
            _accountBalances.Add(new BankAccountBalance {Balance = 0M, Id = eventId, Sequence = sequence});
            return Task.CompletedTask;
        }

        public Task<BankAccountBalance> GetAsync(string eventId)
        {
            return Task.FromResult(_accountBalances.First(b => b.Id == eventId));
        }

        public async Task UpdateBalance(BankAccountBalance balance, long sequence)
        {
            var existingBalance = await GetAsync(balance.Id);
            _accountBalances.Remove(existingBalance);
            _accountBalances.Add(balance);
        }

        public async Task<ICollection<EventEnvelope>> GetEvents(long latestSequence, int eventPageSize)
        {
            var events = new List<EventEnvelope>();

            var eventsToGenerate = _random.Next(1, eventPageSize);

            if (latestSequence == 0)
                events.Add(new EventEnvelope(
                    new BankAccountCreated {AccountHolder = "Tarik Eminagic", Id = BankAccount.ToString()}, 1));

            while (events.Count < eventsToGenerate)
                switch (_random.Next() % 2)
                {
                    case 0:
                        events.Add(new EventEnvelope(
                            new MoneyDeposited
                                {Amount = Convert.ToDecimal(_random.Next(0, 500)), Id = BankAccount.ToString()},
                            events.Count + 1));
                        break;
                    case 1:
                        events.Add(new EventEnvelope(
                            new MoneyWithdrawn
                                {Amount = Convert.ToDecimal(_random.Next(0, 1337)), Id = BankAccount.ToString()},
                            events.Count + 1));
                        break;
                }

            await Task.Delay(TimeSpan.FromSeconds(_random.Next(1, 5)));
            return events;
        }
    }
}