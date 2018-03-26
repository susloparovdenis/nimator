using System;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using log4net;
using Xunit;

namespace Nimator.Checks.Couchbase.IntegrationTests
{
    /// <summary>
    ///     Couchbase is running in docker.
    ///     Database storage location: ./database
    ///     Contains one bucket - gamesim-sample with 586 documents
    /// </summary>
    public class IntegrationTests: IDisposable
    {
        public IntegrationTests()
        {
            DockerCompose("up -d");
        }

        private static void DockerCompose(string param)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = $"-f ../../../../docker-compose.yml {param}"
            };
            var process = Process.Start(processStartInfo);

            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
        }

        [Fact]
        public void IntegrationWithDatabaseAndNimator()
        {
            //Arrange
            var logger = A.Dummy<ILog>();
            var notifier = A.Fake<INotifier>();
            NotifierMockSettings.Notifier = notifier;
            var nimatorSettingsJson = EmbeddedResource.Read("config.json");
            var nimator = Nimator.FromSettings(logger, nimatorSettingsJson);

            INimatorResult nimatorResult = null;
            A.CallTo(() => notifier.Notify(A<INimatorResult>._))
                .Invokes(call => nimatorResult = (INimatorResult) call.Arguments[0]);

            //Act
            nimator.TickSafe(logger);

            //Assert
            nimatorResult.Should().NotBeNull();
            A.CallTo(() => notifier.Notify(A<INimatorResult>._)).MustHaveHappened();
            nimatorResult.Level.Should()
                .Be(NotificationLevel.Warning);
            var layerResult = nimatorResult.LayerResults.Should().ContainSingle().Subject;
            layerResult.CheckResults.Should().HaveCount(2)
                .And.ContainSingle(r => r.CheckName == "DocumentsNumberCheck" && r.Level == NotificationLevel.Warning)
                .And.ContainSingle(r => r.CheckName == "RamUsageCheck" && r.Level == NotificationLevel.Okay);
        }

        public void Dispose()
        {
            DockerCompose("down");
        }
    }
}