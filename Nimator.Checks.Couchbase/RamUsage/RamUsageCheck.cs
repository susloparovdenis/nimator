using System;
using System.Threading.Tasks;
using Nimator.Checks.Couchbase.Utlis;

namespace Nimator.Checks.Couchbase.RamUsage
{
    /// <summary>
    ///     Checks if ram usage above specified level
    /// </summary>
    public class RamUsageCheck : ICheck
    {
        private readonly IRamUsageInfoProvider _provider;
        private readonly RamUsageSettings _settings;

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
                var usage = (double) used / total;
                if (usage < 0 || usage > 1 || double.IsNaN(usage))
                    return new CheckResult(ShortName, NotificationLevel.Error,
                        $"ram info is not correct: {usage}");
                if (usage < _settings.MaxUsage)
                    return new CheckResult(ShortName, NotificationLevel.Okay);

                var notificationLevel = usage < _settings.MaxUsage
                    ? NotificationLevel.Okay
                    : NotificationLevel.Warning;
                return new CheckResult(ShortName, NotificationLevel.Warning,
                    $"RAM usage is: {usage:P}, max is: {_settings.MaxUsage:P}");
            }
            catch (Exception e)
            {
                return new CheckResult(ShortName, NotificationLevel.Error, e.Message);
            }
        }
    }
}