using System.Threading.Tasks;

namespace Nimator.Checks.Couchbase.DocumentsNumber
{
    public interface IBucketInfoProvider
    {
        Task<long> GetDocumentsCount(string bucketName);

    }
}