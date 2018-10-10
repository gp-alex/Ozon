using LumenWorks.Framework.IO.Csv;

namespace HistoricalParserApplication.Services
{
    public class RatesParser
    {
        private readonly ILogger log;
        public RatesParser(ILogger log)
        {
            this.log = log.ForContext(GetType());
            this.parser = new CachedCsvReader();
        }

        public IEnumerable<Symbol>
    }
}
