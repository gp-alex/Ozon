using DomainModels;
using InfrastructureWeb.Models;
using LumenWorks.Framework.IO.Csv;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InfrastructureWeb.Services
{
    internal class PairYearlyParser
    {
        private readonly ILogger log;
        public PairYearlyParser(ILogger log)
        {
            this.log = log?.ForContext(GetType());
        }


        public IEnumerable<Pair> Parse(string csv)
        {
            if (csv == null) throw new ArgumentNullException("csv");

            try
            {
                using (var reader = new StringReader(csv))
                using (var csvProvider = new CachedCsvReader(reader, true, '|'))
                {
                    if (!csvProvider.HasHeaders) throw new ArgumentException("csv");
                    return DoParse(csvProvider);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "CreateFromWebCsv");
                return Array.Empty<Pair>();
            }
        }

        private IEnumerable<Pair> DoParse(CachedCsvReader csv)
        {
            var rates = new List<Pair>();

            var headers = csv.GetFieldHeaders();
            var dateColumnIndex = headers
                .Select((header, idx) => new { idx, header })
                .Where(x => x.header.Equals("Date"))
                .Select(x => x.idx)
                .Single();

            var volumes = headers
                .Select(
                    (header, idx) => KeyValuePair.Create(idx, ParseSymbolVolume(header))
                )
                .Where(
                    x => x.Value != null
                    && double.IsFinite(x.Value.Volume)
                    && Math.Abs(x.Value.Volume) > double.Epsilon
                )
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var line in csv)
            {
                var dt = DateTime.Parse(line[dateColumnIndex]);
                for (int i = 0; i < headers.Length; ++i)
                {
                    if (i == dateColumnIndex)
                    {
                        continue;
                    }

                    if (double.TryParse(
                            line[i],
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out double rate
                        )
                    )
                    {
                        var volume = volumes[i];
                        rates.Add(
                            new Pair()
                            {
                                BaseSymbol = volume.Symbol,
                                CounterSymbol = "CZK",
                                Rate = volume.Volume / rate,
                                Dt = dt
                            }
                        );
                    }
                }
            }

            return rates;
        }

        private SymbolVolume ParseSymbolVolume(string s)
        {
            try
            {
                var match = new Regex(@"^\s*(\d+)\s*(\w+)\s*$").Match(s);
                return new SymbolVolume()
                {
                    Symbol = match.Groups[2].Value,
                    Volume = Convert.ToDouble(match.Groups[1].Value)
                };
            }
            catch (Exception e)
            {
                log.Warning(e, "ParseSymbolVolume");
                return null;
            }
        }
    }
}
