using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TProjection.Pooling;
using TProjections.TestHost.Sample.Projections;
using TProjections.TestHost.Sample.Repositories;

namespace TProjections.TestHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddTransient<IBankAccountBalanceRepository, BankAccountBalanceRepository>();
                    services.AddTransient<IPassiveEventStore, BankAccountBalanceRepository>();
                    services.AddTransient<BankAccountBalanceProjection>();
                    services.AddTransient<IPoolingProjector, PoolingProjector<BankAccountBalanceProjection>>();
                });
        }
    }
}