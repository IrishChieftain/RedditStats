using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;

namespace RedditStats.Services
{
    public class RateLimiter : IRateLimiter
    {
        private int _remainingRequests;
        private DateTime _resetTime;

        public RateLimiter()
        {
            _remainingRequests = int.MaxValue; // Assume maximum until told otherwise.
            _resetTime = DateTime.UtcNow;
        }

        public void UpdateLimits(HttpResponseHeaders headers)
        {
            if (headers.Contains("X-Ratelimit-Remaining"))
            {
                _remainingRequests = int.Parse(headers.GetValues("X-Ratelimit-Remaining").FirstOrDefault()!);
            }

            if (headers.Contains("X-Ratelimit-Reset"))
            {
                int resetInSeconds = int.Parse(headers.GetValues("X-Ratelimit-Reset").FirstOrDefault()!);
                _resetTime = DateTime.UtcNow.AddSeconds(resetInSeconds);
            }
        }

        public bool AllowRequest()
        {
            if (DateTime.UtcNow >= _resetTime)
            {
                _remainingRequests = int.MaxValue; // Reset the count every period as per header
                return true;
            }

            return _remainingRequests > 0;
        }

        public void RequestMade()
        {
            if (_remainingRequests != int.MaxValue)
            {
                _remainingRequests--;
            }
        }

        public void WaitIfNeeded()
        {
            if (_remainingRequests <= 0)
            {
                var waitTime = _resetTime - DateTime.UtcNow;
                if (waitTime.TotalMilliseconds > 0)
                {
                    Thread.Sleep(waitTime);
                }
            }
        }
    }
}
