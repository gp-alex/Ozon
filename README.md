# Ozon

# Configuration 
1. Configure sqlite connection strings in appconfig.InrastructureEf.josn (App & Web projects) to the same database file
2. Configure Console App for years (task 1) in appconfig.json:HistoricalParserApplication:YearsToParse
3. Confugure Web App fetching schedule (task 2) in appsettings.json:RatesFetchSchedule  (crontab format)
4. Configure Web App for repoting symbols (task 3) in appsettings.json:ApiControlerOptions:RequiredSymbols

# Run
1. Run Console App (HistoricalParserApplication) for initial seed 
2. Run Web App (WebApiApplication) for reporting; example:
* http://localhost/api/report/2018/01.txt
* http://localhost/api/report/2018/01.json
