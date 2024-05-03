using Microsoft.AspNetCore.Mvc;
using RedditStats.Services;

namespace RedditStats.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _redditService;
        private readonly ILogger<RedditController> _logger;

        public RedditController(IRedditService redditService, ILogger<RedditController> logger)
        {
            _redditService = redditService ?? throw new ArgumentNullException(nameof(redditService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{subreddit}")]
        public async Task<IActionResult> GetPosts(string subreddit)
        {
            try
            {
                var posts = await _redditService.GetPosts(subreddit);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching posts from Reddit."); 
                throw;
            }
        }
    }
}
