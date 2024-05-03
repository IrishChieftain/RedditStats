using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RedditStats.API.Controllers;
using RedditStats.Models;
using RedditStats.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace RedditStats.Tests.IntegrationTests
{
    public class RedditControllerTest
    {
        private readonly RedditController _controller;

        public RedditControllerTest()
        {
            // Create a mock logger
            var mockLogger = new Mock<ILogger<RedditController>>();

            // Create a mock Reddit service that returns mock posts
            var mockRedditService = new Mock<IRedditService>();

            mockRedditService.Setup(service => service.GetPosts(It.IsAny<string>()))
                             .ReturnsAsync(GetMockPosts()); // Return mock posts

            // Inject the mock Reddit service into the controller
            _controller = new RedditController(mockRedditService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetPosts_ReturnsOkResult_WithPosts()
        {
            // Arrange
            var subreddit = "technology";

            // Act
            var result = await _controller.GetPosts(subreddit);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var redditPosts = Assert.IsAssignableFrom<IEnumerable<RedditPost>>(okResult.Value);
            Assert.NotEmpty(redditPosts);
        }

        private IEnumerable<RedditPost> GetMockPosts()
        {
            return new List<RedditPost>
            {
                new RedditPost
                {
                    Id = "1",
                    Title = "Post 1",
                    Author = "Author 1",
                    Upvotes = 100
                },
                new RedditPost
                {
                    Id = "2",
                    Title = "Post 2",
                    Author = "Author 2",
                    Upvotes = 200
                },
                new RedditPost
                {
                    Id = "3",
                    Title = "Post 3",
                    Author = "Author 3",
                    Upvotes = 300
                },
                new RedditPost
                {
                    Id = "4",
                    Title = "Post 4",
                    Author = "Author 4",
                    Upvotes = 400
                },
                 new RedditPost
                {
                    Id = "5",
                    Title = "Post 5",
                    Author = "Author 5",
                    Upvotes = 500
                }
            };
        }
    }
}
