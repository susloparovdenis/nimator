using System.Threading.Tasks;

namespace Nimator.Checks.Couchbase
{
    public interface ICouchbaseInfoClient
    {
        Task<(long used, long total)> GetRamInfo();
        Task<long> GetDocumentsCount();
    }
}