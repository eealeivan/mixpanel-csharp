using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Mixpanel.Tests.Integration
{
    public class DefaultHttpClientTests
    {
        private const string BaseUrl = "https://eealeivan-mixpanel.free.beeceptor.com/";

        [Test]
        public async Task PostAsync_SuccessRequest_TrueReturned()
        {
            // Arrange
            var httpClient = new DefaultHttpClient();

            // Act
            var result = await httpClient.PostAsync(BaseUrl + "success", "", CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task PostAsync_FailRequest_FalseReturned()
        {
            // Arrange
            var httpClient = new DefaultHttpClient();

            // Act
            var result = await httpClient.PostAsync(BaseUrl + "fail", "", CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task PostAsync_CancellationToken_CancelledForSlowRequest()
        {
            // Arrange
            var httpClient = new DefaultHttpClient();
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            Func<Task> act = async () => { await httpClient.PostAsync(BaseUrl + "success-5000ms", "", cancellationToken); };
            
            // Assert
            await act.Should().ThrowAsync<TaskCanceledException>();
        }
    }
}