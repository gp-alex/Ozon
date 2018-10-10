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
            this.requestedYears = requestedYears ?? throw new ArgumentNullException("requestedYears");
            this.remotePairRepository = remotePairRepository ?? throw new ArgumentNullException("remotePairRepository");
            this.persistancePairRepository = persistancePairRepository ?? throw new ArgumentNullException("persistancePairRepository");
            this.log = log?.ForContext(GetType());
        }

        public void Run()
        {
            var tasks = requestedYears.Select(
                year => remotePairRepository.FindForYearAsync(year)
            ).ToArray();
            Task.WaitAll(tasks);
            var rates = tasks.SelectMany(t => t.Result).ToList();

            Task.Run(
                () => persistancePairRepository.SaveAllAsync(rates)
            ).GetAwaiter().GetResult();
        }
    }
}
