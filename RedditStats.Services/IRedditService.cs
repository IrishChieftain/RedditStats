using RedditStats.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditStats.Services
{
    public interface IRedditService
    {
        Task<IEnumerable<RedditPost>> GetPosts(string subreddit);
    }
}
