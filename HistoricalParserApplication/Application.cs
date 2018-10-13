using Infrastructure;
using InfrastructureWeb.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HistoricalParserApplication
{
    public class Application
    {
        private IEnumerable<int> requestedYears { get; }
        private readonly WebPairRepository remotePairRepository;
        private readonly IPairRepository persistancePairRepository;
        private readonly ILogger log;
        public Application(
            IEnumerable<int> requestedYears,
            WebPairRepository remotePairRepository,
            IPairRepository persistancePairRepository,
            ILogger log
        )
        {
            this.requestedYears = requestedYears ?? throw new ArgumentNullException(nameof(requestedYears));
            this.remotePairRepository = remotePairRepository ?? throw new ArgumentNullException(nameof(remotePairRepository));
            this.persistancePairRepository = persistancePairRepository ?? throw new ArgumentNullException(nameof(persistancePairRepository));
            this.log = log?.ForContext(GetType());
        }

        public async Task RunAsync()
        {
            var tasks = requestedYears.Select(
                async year => await remotePairRepository.FindForYearAsync(year)
            );
            var rates = (await Task.WhenAll(tasks))
                .SelectMany(res => res)
                .ToList();

            await persistancePairRepository.DeleteAllAsync();
            await persistancePairRepository.SaveAllAsync(rates);
        }

        public void Run()
        {
            Task.WaitAll(
                Task.Run(() => RunAsync())
            );
        }
    }
}
