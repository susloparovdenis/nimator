using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Nimator.Checks.Couchbase.DocumentsNumber;
using Nimator.Checks.Couchbase.RamUtilization;
using Nimator.Checks.Couchbase.Utlis;

namespace Nimator.Checks.Couchbase.Connection
{
    public class CouchbaseClient: IBucketInfoProvider, IRamUsageInfoProvider
    {
        private readonly ClientConfiguration _clientConfiguration;
        private readonly ConnectionSettings _connectionSettings;

        public CouchbaseClient(ConnectionSettings settings)
        {
            Ensure.ArgumentNotNull(settings, nameof(settings));
            _connectionSettings = settings;
            _clientConfiguration = new ClientConfiguration()
            {
                Servers = new List<Uri> { new Uri(_connectionSettings.Server) }
            };
        }

        public async Task<(long used, long total)> GetRamInfo()
        {
            using (var cluster = new Cluster(_clientConfiguration))
            {
                var clusterManager = cluster.CreateManager(_connectionSettings.Login, _connectionSettings.Password);
                var clusterInfo = await clusterManager.ClusterInfoAsync();
                var ram = clusterInfo.Value.Pools().StorageTotals.Ram;
                return (ram.Used, ram.Total);
            }
        }

        public async Task<long> GetDocumentsCount(string bucketName)
        {
            using (var cluster = new Cluster(_clientConfiguration))
            {
                var clusterManager = cluster.CreateManager(_connectionSettings.Login, _connectionSettings.Password);
                var clusterInfo = await clusterManager.ClusterInfoAsync();
                return clusterInfo.Value.BucketConfigs().Single(b => b.Name == bucketName).BasicStats.ItemCount;
            }
        }
    }
}
