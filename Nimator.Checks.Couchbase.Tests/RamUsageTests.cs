using System;
using FakeItEasy;
using FluentAssertions;
using Nimator.Checks.Couchbase.RamUtilization;
using Xunit;

namespace Nimator.Checks.Couchbase.Tests
{
    public class RamUsageTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_GivenSettingsIsNull()
        {
            //Arrange
            IRamUsageInfoProvider provider = null;
            //Act
            void Act() => new RamUsageCheck(provider, null);

            //Assert
            Assert.Throws<ArgumentNullException>((Action) Act);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_GivenProviderIsNull()
        {
            //Arrange
            IRamUsageInfoProvider provider = null;
            //Act
            void Act() => new RamUsageCheck(null, new RamUsageSettings());

            //Assert
            Assert.Throws<ArgumentNullException>((Action)Act);
        }

        [Theory]
        [InlineData(16L, 100L, NotificationLevel.Warning)]
        [InlineData(14L, 100L, NotificationLevel.Okay)]
        [InlineData(14L, 0L, NotificationLevel.Error)]
        [InlineData(0L, 0L, NotificationLevel.Error)]
        [InlineData(100L, 10L, NotificationLevel.Error)]
        public void RunAsync_ReturnsCorrectNotificationLevel(long used, long total, NotificationLevel expected)
        {
            //Arrange
            var provider = A.Fake<IRamUsageInfoProvider>();
            A.CallTo(() => provider.GetRamInfo()).Returns((used, total));
            var memoryUtilizationCheck = new RamUsageCheck(provider, new RamUsageSettings());

            //Act
            var checkResult = memoryUtilizationCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(expected);
        }



        [Fact]
        public void RunAsync_ReturnsError_GivenExceptionInCouchbaseClient()
        {
            //Arrange
            var provider = A.Fake<IRamUsageInfoProvider>();
            A.CallTo(() => provider.GetRamInfo()).Throws<Exception>();
            var memoryUtilizationCheck = new RamUsageCheck(provider, new RamUsageSettings());

            //Act
            var checkResult = memoryUtilizationCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(NotificationLevel.Error);
        }
    }
}