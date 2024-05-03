using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditStats.Models;

namespace RedditStats.Services
{
    public class RedditService : IRedditService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RedditService> _logger;
        private readonly IRateLimiter _rateLimiter;
        private readonly RedditApiSettings _redditApiSettings;

        private List<RedditPost> _postsWithMostUpvotes = new List<RedditPost>();
        private Dictionary<string, int> _userPostCounts = new Dictionary<string, int>();

        public RedditService(HttpClient httpClient, ILogger<RedditService>? logger, IRateLimiter? rateLimiter, 
            IOptions<RedditApiSettings> redditApiSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
            _redditApiSettings = redditApiSettings?.Value ?? throw new ArgumentNullException(nameof(redditApiSettings));

            _httpClient.BaseAddress = new Uri(_redditApiSettings.SubredditUrl!);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _redditApiSettings.UserAgent);
        }

        public async Task<IEnumerable<RedditPost>> GetPosts(string subreddit)
        {
            try
            {
                // Use RateLimiter to control throughput
                if (!_rateLimiter.AllowRequest())
                {
                    // Log rate limiting
                    _logger.LogWarning("Rate limit exceeded. Waiting before making another request.");
                    await Task.Delay(TimeSpan.FromSeconds(10)); // Wait for 10 seconds before retrying
                }

                var response = await _httpClient.GetAsync($"https://www.reddit.com/r/technology/new.json");
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response to extract Reddit posts
                var redditPosts = ParseRedditPosts(responseBody);

                // Update and report statistics
                UpdatePostStatistics(redditPosts);
                UpdateUserStatistics(redditPosts);
                LogStatistics();

                return redditPosts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching posts from Reddit.");
                throw;
            }
        }

        private IEnumerable<RedditPost> ParseRedditPosts(string responseBody)
        {
            var redditPosts = new List<RedditPost>();

            using (JsonDocument document = JsonDocument.Parse(responseBody))
            {
                if (document.RootElement.TryGetProperty("data", out JsonElement dataElement) &&
                    dataElement.TryGetProperty("children", out JsonElement childrenElement) &&
                    childrenElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement childElement in childrenElement.EnumerateArray())
                    {
                        if (childElement.TryGetProperty("data", out JsonElement postData) && postData.ValueKind == JsonValueKind.Object)
                        {
                            var id = postData.TryGetProperty("id", out JsonElement idElement) ? idElement.GetString() : null;
                            var title = postData.TryGetProperty("title", out JsonElement titleElement) ? titleElement.GetString() : null;
                            var author = postData.TryGetProperty("author", out JsonElement authorElement) ? authorElement.GetString() : null;
                            var upvotes = postData.TryGetProperty("upvotes", out JsonElement upvotesElement) ? upvotesElement.GetInt32() : 0;

                            redditPosts.Add(new RedditPost
                            {
                                Id = id,
                                Title = title,
                                Author = author,
                                Upvotes = upvotes,
                            });
                        }
                    }
                }
            }

            return redditPosts;
        }

        private void UpdatePostStatistics(IEnumerable<RedditPost> posts)
        {
            foreach (var post in posts)
            {
                if (_postsWithMostUpvotes.Count < 5)
                {
                    _postsWithMostUpvotes.Add(post);
                }
                else
                {
                    var minUpvotesPost = _postsWithMostUpvotes.OrderBy(p => p.Upvotes).First();
                    if (post.Upvotes > minUpvotesPost.Upvotes)
                    {
                        _postsWithMostUpvotes.Remove(minUpvotesPost);
                        _postsWithMostUpvotes.Add(post);
                    }
                }
            }
        }

        private void UpdateUserStatistics(IEnumerable<RedditPost> posts)
        {
            // Update user post counts
            foreach (var post in posts)
            {
                if (_userPostCounts.ContainsKey(post.Author!))
                {
                    _userPostCounts[post.Author!]++;
                }
                else
                {
                    _userPostCounts[post.Author!] = 1;
                }
            }
        }

        private void LogStatistics()
        {
            if (_postsWithMostUpvotes.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Posts with most upvotes:");
                foreach (var post in _postsWithMostUpvotes.OrderByDescending(p => p.Upvotes))
                {
                    Console.WriteLine($"{post.Title}: {post.Upvotes} upvotes");
                }
            }
            else
            {
                _logger.LogInformation("No posts with upvotes found.");
            }

            // Log users with most posts
            var topUsers = _userPostCounts.OrderByDescending(kv => kv.Value).Take(5);
            Console.WriteLine();
            Console.WriteLine("Top users by post count:");
            foreach (var user in topUsers)
            {
                Console.WriteLine($"{user.Key}: {user.Value} posts");
            }
        }
    }
}
