using System.Threading;
using System.Threading.Tasks;

namespace WebApiApplication.AspInfrastructure.HostingServices
{
    public interface IScheduledTask
    {
        string Schedule { get; }
    }
}
