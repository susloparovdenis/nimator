using Nimator.Settings;

namespace Nimator.Checks.Couchbase.IntegrationTests
{
    internal class NotifierMockSettings : NotifierSettings
    {
        public static INotifier Notifier { get; set; }

        public override INotifier ToNotifier() => Notifier;
    }
}