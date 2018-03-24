using System;
using Couchbase.Configuration.Client;
using FluentAssertions;
using Nimator.Checks.Couchbase.Connection;
using Nimator.Checks.Couchbase.DocumentsNumber;
using Nimator.Checks.Couchbase.RamUtilization;
using Xunit;

namespace Nimator.Checks.Couchbase.IntegrationTests
{
    /// <summary>
    ///  Couchbase is running in docker.
    ///  Database storage location: ./database
    ///  Contains one bucket - gamesim-sample with 586 documents
    /// </summary>
    public class DocumentsNumberTests
    {
        [Fact]
        public void Should_ReturnWarningFor()
        {
            //Arrange
            var settings = new DocumentsNumberCheckSettings
            {
                Server = "http://127.0.0.1:8091",
                Login = "admin",
                Password = "23461234623",
                Bucket = "gamesim-sample",
                MaxDocumentNumber = 500
            };

            var memoryUsageCheck = new DocumentsNumberCheck(new CouchbaseClient(settings), settings);

            //Act
            var checkResult = memoryUsageCheck.RunAsync().Result;

            //Assert
            checkResult.Level.Should().Be(NotificationLevel.Warning);
        }
    }
}
