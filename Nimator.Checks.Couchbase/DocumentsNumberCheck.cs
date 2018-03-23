using System;
using System.Threading.Tasks;
using Nimator.Checks.Couchbase.Utlis;

namespace Nimator.Checks.Couchbase
{
    public class DocumentsNumberCheck : ICheck
    {
        private readonly ICouchbaseInfoClient _client;

        public DocumentsNumberCheck(ICouchbaseInfoClient client)
        {
            Ensure.ArgumentNotNull(client, nameof(client));
            _client = client;
        }

        public int MaxDocumentNumber { get; set; } = 100000;

        public string ShortName => nameof(DocumentsNumberCheck);

        public async Task<ICheckResult> RunAsync()
        {
            try
            {
                var number = await _client.GetDocumentsCount();
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