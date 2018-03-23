using System;
using System.Threading.Tasks;
using Nimator.Checks.Couchbase.Utlis;

namespace Nimator.Checks.Couchbase.RamUtilization
{
    /// <summary>
    /// Checks if ram utilization above specified level
    /// </summary>
    public class RamUsageCheck : ICheck
    {
        private readonly RamUsageSettings _settings;
        private readonly IRamUsageInfoProvider _provider;

        public RamUsageCheck(IRamUsageInfoProvider provider, RamUsageSettings settings)
        {
            Ensure.ArgumentNotNull(provider, nameof(provider));
            Ensure.ArgumentNotNull(settings, nameof(settings));
            _settings = settings;
            _provider = provider;
        }

        public string ShortName => nameof(RamUsageCheck);

        public async Task<ICheckResult> RunAsync()
        {
            try
            {
                (var used, var total) = await _provider.GetRamInfo();
                var utilization = (double)used / total;
                if (utilization < 0 || utilization > 1 || double.IsNaN(utilization))
                    return new CheckResult(ShortName, NotificationLevel.Error,
                        $"ram utilization is not correct: {utilization}");
                if(utilization < _settings.MaxMemoryUtilization)
                    return new CheckResult(ShortName, NotificationLevel.Okay);

                var notificationLevel = utilization < _settings.MaxMemoryUtilization
                    ? NotificationLevel.Okay
                    : NotificationLevel.Warning;
                return new CheckResult(ShortName, NotificationLevel.Warning, $"RAM utilization is: {utilization:P}, max is: {_settings.MaxMemoryUtilization:P}");
            }
            catch (Exception e)
            {
                return new CheckResult(ShortName, NotificationLevel.Error, e.Message);
            }
        }
    }
}