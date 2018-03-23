using System;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Nimator.Checks.Couchbase.Tests
{
    public class MemoryUtilizationCheckTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_GivenNullArgument()
        {
            //Arrange

            //Act
            void Act() => new MemoryUtilizationCheck(null);

            //Assert
            Assert.Throws<ArgumentNullException>((Action) Act);
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
            var couchbaseInfoClient = A.Fake<ICouchbaseInfoClient>();
            A.CallTo(() => couchbaseInfoClient.GetRamInfo()).Returns((used, total));
            var memoryUtilizationCheck = new MemoryUtilizationCheck(couchbaseInfoClient);

            //Act
            var checkResult = memoryUtilizationCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(expected);
        }



        [Fact]
        public void RunAsync_ReturnsError_GivenExceptionInCouchbaseClient()
        {
            //Arrange
            var couchbaseInfoClient = A.Fake<ICouchbaseInfoClient>();
            A.CallTo(() => couchbaseInfoClient.GetRamInfo()).Throws<Exception>();
            var memoryUtilizationCheck = new MemoryUtilizationCheck(couchbaseInfoClient);

            //Act
            var checkResult = memoryUtilizationCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(NotificationLevel.Error);
        }
    }
}