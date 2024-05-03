using System.Net.Http.Headers;

namespace RedditStats.Services
{
    public interface IRateLimiter
    {
        void UpdateLimits(HttpResponseHeaders headers);
        bool AllowRequest();
        void RequestMade();
        void WaitIfNeeded();
    }
}