using System;
using System.Threading.Tasks;
using Nimator.Checks.Couchbase.Connection;
using Nimator.Checks.Couchbase.Utlis;

namespace Nimator.Checks.Couchbase.DocumentsNumber
{
    /// <summary>
    /// Checks that the number of documents in a bucket doesn't exceed some level
    /// </summary>
    public class DocumentsNumberCheck : ICheck
    {
        private readonly IBucketInfoProvider _provider;
        private readonly DocumentsNumberCheckSettings _settings;
        public DocumentsNumberCheck(IBucketInfoProvider provider, DocumentsNumberCheckSettings settings)
        {
            Ensure.ArgumentNotNull(provider, nameof(provider));
            Ensure.ArgumentNotNull(settings, nameof(settings));
            _provider = provider;
            _settings = settings;
        }

        public int MaxDocumentNumber { get; set; } = 100000;

        public string ShortName => nameof(DocumentsNumberCheck);

        public async Task<ICheckResult> RunAsync()
        {
            try
            {
                var number = await _provider.GetDocumentsCount(_settings.Bucket);
                return number > MaxDocumentNumber 
                    ? new CheckResult(ShortName, NotificationLevel.Warning) 
                    : new CheckResult(ShortName, NotificationLevel.Okay);
            }
            catch (Exception e)
            {
                return new CheckResult(ShortName, NotificationLevel.Error, e.Message);
            }
        }
    }
}