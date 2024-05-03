namespace RedditStats.Models
{
    public  class RedditPost
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public int Upvotes { get; set; }
        public string? Author { get; set; }
    }
}
