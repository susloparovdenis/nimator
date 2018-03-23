using Nimator.Checks.Couchbase.Connection;

namespace Nimator.Checks.Couchbase.RamUtilization
{
    public class RamUsageSettings : ConnectionSettings, ICheckSettings
    {
        /// <summary>
        /// Percentage. Float number from 0 to 1
        /// </summary>
        public double MaxMemoryUtilization { get; set; } = 0.15;

        public ICheck ToCheck()
        {
            var provider = new CouchbaseClient(this);
            return new RamUsageCheck(provider, this);
        }
    }
}