using System.Threading.Tasks;

namespace Nimator.Checks.Couchbase.RamUtilization
{
    public interface IRamUsageInfoProvider
    {
        Task<(long used, long total)> GetRamInfo();

    }
}