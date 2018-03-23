using System;
using FakeItEasy;
using FluentAssertions;
using Nimator.Checks.Couchbase.DocumentsNumber;
using Xunit;

namespace Nimator.Checks.Couchbase.Tests
{
    public class DocumentsNumberTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_GivenSettingsIsNull()
        {
            //Arrange
            IBucketInfoProvider provider = null;
            //Act
            void Act() => new DocumentsNumberCheck(provider, null);

            //Assert
            Assert.Throws<ArgumentNullException>((Action)Act);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_GivenProviderIsNull()
        {
            //Arrange
            IBucketInfoProvider provider = null;
            //Act
            void Act() => new DocumentsNumberCheck(null, new DocumentsNumberCheckSettings());

            //Assert
            Assert.Throws<ArgumentNullException>((Action)Act);
        }

        [Theory]
        [InlineData(100001L, NotificationLevel.Warning)]
        [InlineData(10L, NotificationLevel.Okay)]
        public void RunAsync_ReturnsCorrectNotificationLevel(long number, NotificationLevel expected)
        {
            //Arrange
            var settings = new DocumentsNumberCheckSettings(){Bucket = "bucketName" };
            var provider = A.Fake<IBucketInfoProvider>();
            A.CallTo(() => provider.GetDocumentsCount(settings.Bucket)).Returns(number);
            var documentsNumberCheck = new DocumentsNumberCheck(provider, settings);

            //Act
            var checkResult = documentsNumberCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(expected);
        }
        
        [Fact]
        public void RunAsync_ReturnsError_GivenExceptionInCouchbaseClient()
        {
            //Arrange
            var settings = new DocumentsNumberCheckSettings() { Bucket = "bucketName" };
            var provider = A.Fake<IBucketInfoProvider>();
            A.CallTo(() => provider.GetDocumentsCount(settings.Bucket)).Throws<Exception>();
            var memoryUtilizationCheck = new DocumentsNumberCheck(provider, settings);

            //Act
            var checkResult = memoryUtilizationCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(NotificationLevel.Error);
        }
    }
}