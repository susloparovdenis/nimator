using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Server.Serialization;

namespace Nimator.Checks.Couchbase
{
    class CouchbaseInfoClient: ICouchbaseInfoClient
    {
        private readonly ClientConfiguration _clientConfiguration;
        private readonly string _login;
        private readonly string _password;

        public CouchbaseInfoClient(string server, string login, string password)
        {
            _clientConfiguration = new ClientConfiguration()
            {
                Servers = new List<Uri> { new Uri(server) }
            };
            _login = login;
            _password = password;
        }

        public async Task<(long used, long total)> GetRamInfo()
        {
            using (var cluster = new Cluster(_clientConfiguration))
            {
                var clusterManager = cluster.CreateManager(_login, _password);
                var clusterInfo = await clusterManager.ClusterInfoAsync();
                var ram = clusterInfo.Value.Pools().StorageTotals.Ram;
                return (ram.Used, ram.Total);
            }
        }

        public async Task<long> GetDocumentsCount()
        {
            using (var cluster = new Cluster(_clientConfiguration))
            {
                var clusterManager = cluster.CreateManager(_login, _password);
                var clusterInfo = await clusterManager.ClusterInfoAsync();
                return clusterInfo.Value.BucketConfigs().First().BasicStats.ItemCount;
            }
        }
    }
}
