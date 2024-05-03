using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using RedditStats.Models;
using RedditStats.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditStats.Tests.UnitTests
{
    public class RedditClientTests
    {
        [Fact]
        public void TestConnection()
        {
            // Arrange
            var mock = new Mock<IRateLimiter>();
            mock.Setup(x => x.AllowRequest()).Returns(true);

            // Act
            bool result = mock.Object.AllowRequest();

            // Assert
            result.Should().BeTrue();
        }

        //[Fact]
        //public async Task GetSubredditDataAsync_ReturnsData()
        //{
        //    // Arrange
        //    var subreddit = "technology";
        //    var expectedData = new List<RedditPost> { new RedditPost { Id = "Test", Title = "Test", Author = "Test", Upvotes = 123 } };

        //    var mockResponseObject = new
        //    {
        //        data = new
        //        {
        //            children = expectedData.Select(x => new { data = x }).ToArray()
        //        }
        //    };

        //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //    mockHttpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(JsonConvert.SerializeObject(mockResponseObject), Encoding.UTF8, "application/json")
        //        });

        //    var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        //    var redditApiSettings = new RedditApiSettings
        //    {
        //        SubredditUrl = "https://www.reddit.com/r/technology/new.json",
        //        UserAgent = "Your User Agent"
        //    };
        //    var mockOptions = new Mock<IOptions<RedditApiSettings>>();
        //    mockOptions.Setup(o => o.Value).Returns(redditApiSettings);

        //    var mockLogger = new Mock<ILogger<RedditService>>();

        //    var mockRateLimiter = new Mock<IRateLimiter>();
        //    mockRateLimiter.Setup(r => r.AllowRequest()).Returns(true);

        //    var redditClient = new RedditService(httpClient, mockLogger.Object, mockRateLimiter.Object, mockOptions.Object);

        //    // Act
        //    var result = await redditClient.GetPosts(subreddit);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeEquivalentTo(expectedData);
        //}

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _expectedData;

            public MockHttpMessageHandler(string expectedData)
            {
                _expectedData = expectedData;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_expectedData)
                };

                return await Task.FromResult(response);
            }
        }
    }
}
