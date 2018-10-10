using DomainModels;
using Infrastructure;
using InfrastructureWeb.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfrastructureWeb.Repositories
{
    public class WebPairRepository : IPairRepository
    {
        private string DailyRatesEndpoint { get; }
        private string YearlyRatesEndpoint { get; }
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger log;
        public WebPairRepository(
            string DailyRatesEndpoint,
            string YearlyRatesEndpoint,
            IHttpClientFactory clientFactory,
            ILogger log
        )
        {
            this.DailyRatesEndpoint = DailyRatesEndpoint ?? throw new ArgumentNullException("DailyRatesEndpoint");
            this.YearlyRatesEndpoint = YearlyRatesEndpoint ?? throw new ArgumentNullException("YearlyRatesEndpoint");
            this.clientFactory = clientFactory ?? throw new ArgumentNullException("clientFactory");
            this.log = log?.ForContext(GetType());
        }

        public async Task<IEnumerable<Pair>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveAllAsync(IEnumerable<Pair> pairs)
            => throw new InvalidOperationException("Saving to a read-only repository");

        public async Task<IEnumerable<Pair>> FindForYearAsync(int year)
        {
            var httpClient = clientFactory.CreateClient();
            var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"{YearlyRatesEndpoint}?year={year}"
            );
            httpRequest.Headers.Add("Accept", "text/plain");
            httpRequest.Headers.Add("User-Agent", "System.Net.Http.HttpClient");

            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return new PairFactory(log).CreateFromWebCsv(responseBody);
        }
    }
}
