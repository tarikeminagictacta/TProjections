using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TProjections.Core;
using TProjections.TestHost.Sample.Events;
using TProjections.TestHost.Sample.Repositories;

namespace TProjections.TestHost.Sample.Projections
{
    public class BankAccountBalanceProjection : Projection
    {
        private readonly ILogger<BankAccountBalanceProjection> _logger;
        private readonly IBankAccountBalanceRepository _repository;

        public BankAccountBalanceProjection(IBankAccountBalanceRepository repository,
            ILogger<BankAccountBalanceProjection> logger = null)
            : base(repository, logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task On(long sequence, BankAccountCreated @event)
        {
            await _repository.RegisterAsync(@event.Id, sequence);
            _logger?.LogInformation($"Event {@event.GetType().Name} Applied.");
        }

        public async Task On(long sequence, MoneyWithdrawn @event)
        {
            var balance = await _repository.GetAsync(@event.Id);
            balance.Balance -= @event.Amount;
            await _repository.UpdateBalance(balance, sequence);
            _logger?.LogInformation($"[{@event.GetType().Name}] New Balance: ${balance.Balance}");
        }

        public async Task On(long sequence, MoneyDeposited @event)
        {
            var balance = await _repository.GetAsync(@event.Id);
            balance.Balance += @event.Amount;
            await _repository.UpdateBalance(balance, sequence);
            _logger?.LogInformation($"[{@event.GetType().Name}] New Balance: ${balance.Balance}");
        }
    }
}