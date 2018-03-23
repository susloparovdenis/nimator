using System;
using System.Threading.Tasks;
using Nimator.Checks.Couchbase.Utlis;

namespace Nimator.Checks.Couchbase
{
    public class MemoryUtilizationCheck : ICheck
    {
        private readonly ICouchbaseInfoClient _client;

        public MemoryUtilizationCheck(ICouchbaseInfoClient client)
        {
            Ensure.ArgumentNotNull(client, nameof(client));
            _client = client;
        }

        public double MinMemoryUtilization { get; set; } = 0.15;

        public string ShortName => nameof(MemoryUtilizationCheck);

        public async Task<ICheckResult> RunAsync()
        {
            try
            {
                (var used, var total) = await _client.GetRamInfo();
                var utilization = (double) used / total;
                if (utilization < 0 || utilization > 1 || double.IsNaN(utilization))
                    return new CheckResult(ShortName, NotificationLevel.Error,
                        $"ram utilization is not correct: {utilization}");
                var notificationLevel = utilization < MinMemoryUtilization
                    ? NotificationLevel.Okay
                    : NotificationLevel.Warning;
                return new CheckResult(ShortName, notificationLevel);
            }
            catch (Exception e)
            {
                return new CheckResult(ShortName, NotificationLevel.Error, e.Message);
            }
        }
    }
}