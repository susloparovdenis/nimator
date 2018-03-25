using System.Threading.Tasks;

namespace Nimator.Checks.Couchbase.RamUsage
{
    public interface IRamUsageInfoProvider
    {
        Task<(long used, long total)> GetRamInfo();
    }
}