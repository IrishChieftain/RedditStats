# Reddit Stats App

## Project Goal
This app should consume the posts from your chosen subreddit in near real time and keep track of the following statistics between the time your application starts until it ends:

•	Posts with most up votes

•	Users with most posts

The app should also provide some way to report these values to a user (periodically log to terminal, return from RESTful web service, etc.).

To acquire near real time statistics from Reddit, we need to continuously request data from Reddit's rest APIs. Reddit implements rate limiting and provides details regarding rate limit used, rate limit remaining, and rate limit reset period via response headers. The application should use these values to control throughput in an even and consistent manner while utilizing a high percentage of the available request rate.

## OUTPUT
![output](https://github.com/IrishChieftain/RedditStats/assets/1145366/160aa191-e5df-4553-aeb4-dc6e82986b8f)
