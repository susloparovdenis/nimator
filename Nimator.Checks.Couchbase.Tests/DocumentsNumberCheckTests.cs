using System;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Nimator.Checks.Couchbase.Tests
{
    public class DocumentsNumberCheckTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_GivenNullArgument()
        {
            //Arrange

            //Act
            void Act() => new DocumentsNumberCheck(null);

            //Assert
            Assert.Throws<ArgumentNullException>((Action)Act);
        }

        [Theory]
        [InlineData(100001L, NotificationLevel.Warning)]
        [InlineData(10L, NotificationLevel.Okay)]
        public void RunAsync_ReturnsCorrectNotificationLevel(long number, NotificationLevel expected)
        {
            //Arrange
            var couchbaseInfoClient = A.Fake<ICouchbaseInfoClient>();
            A.CallTo(() => couchbaseInfoClient.GetDocumentsCount()).Returns(number);
            var documentsNumberCheck = new DocumentsNumberCheck(couchbaseInfoClient);

            //Act
            var checkResult = documentsNumberCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(expected);
        }
        
        [Fact]
        public void RunAsync_ReturnsError_GivenExceptionInCouchbaseClient()
        {
            //Arrange
            var couchbaseInfoClient = A.Fake<ICouchbaseInfoClient>();
            A.CallTo(() => couchbaseInfoClient.GetDocumentsCount()).Throws<Exception>();
            var memoryUtilizationCheck = new DocumentsNumberCheck(couchbaseInfoClient);

            //Act
            var checkResult = memoryUtilizationCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(NotificationLevel.Error);
        }
    }
}