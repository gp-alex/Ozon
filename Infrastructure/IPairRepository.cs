namespace Infrastructure
{
    public class IPairRepository
    {
        public IPairRepository(ILogger log)
        {
            this.log = log.ForContext(GetType());
        }

        public IQueryable<Pair>
    }
}
