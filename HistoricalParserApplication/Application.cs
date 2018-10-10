namespace HistoricalParserApplication
{
    public class Application
    {
        public Application(ILogger log)
        {
            this.log = log.ForContext(GetType());
        }

        public void Run()
        {

        }
    }
}
