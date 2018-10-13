using DomainModels;
using LumenWorks.Framework.IO.Csv;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace InfrastructureWeb.Services
{
    public class PairDailyParser
    {
        private readonly ILogger log;
        public PairDailyParser(ILogger log)
        {
            this.log = log?.ForContext(GetType());
        }

        public IEnumerable<Pair> Parse(string csv)
        {
            if (csv == null) throw new ArgumentNullException("csv");

            try
            {
                using (var reader = new StringReader(csv))
                {
                    var dt = ParseDailyDateLine(reader.ReadLine());

                    using (var csvProvider = new CachedCsvReader(reader, true, '|'))
                    {
                        if (!csvProvider.HasHeaders) throw new ArgumentException("csv");
                        return DoParse(csvProvider, dt);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "CreateFromWebCsvDaily");
                return Array.Empty<Pair>();
            }
        }

        private DateTime ParseDailyDateLine(string s)
        {
            return DateTime.ParseExact(
                s.Split(" #")[0],
                "dd.MMM yyyy",
                CultureInfo.InvariantCulture
            );
        }

        private IEnumerable<Pair> DoParse(CachedCsvReader csv, DateTime dt)
        {
            var pairs = new List<Pair>();

            var headers = csv.GetFieldHeaders();
            var symbolColumnIndex = headers
                .Select((header, idx) => new { idx, header })
                .Where(x => x.header.Equals("Code"))
                .Select(x => x.idx)
                .Single();
            var rateColumnIndex = headers
                .Select((header, idx) => new { idx, header })
                .Where(x => x.header.Equals("Rate"))
                .Select(x => x.idx)
                .Single();
            var rates = csv.Select(
                line => new Pair()
                {
                    Dt = dt,
                    BaseSymbol = "CZK",
                    CounterSymbol = line[symbolColumnIndex],
                    Rate = ParseDoubleInverted(line[rateColumnIndex])
                }
            ).Where(
                x => !double.IsNaN(x.Rate)
            );

            return rates.ToArray();
        }

        private double ParseDoubleInverted(string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
                && Math.Abs(d) > double.Epsilon
            )
            {
                return 1.0 / d;
            }
            return double.NaN;
        }
    }
}
