using Nimator.Checks.Couchbase.Connection;

namespace Nimator.Checks.Couchbase.DocumentsNumber
{
    public class DocumentsNumberCheckSettings : ConnectionSettings, ICheckSettings
    {
        public int? MaxDocumentNumber { get; set; } = 1000;

        public string Bucket { get; set; }

        public ICheck ToCheck()
        {
            var provider = new CouchbaseClient(this);
            return new DocumentsNumberCheck(provider, this);
        }
    }
}