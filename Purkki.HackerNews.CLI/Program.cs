using System;
using System.Threading.Tasks;

namespace Purkki.HackerNews.CLI
{
    class Program
    {
        static async Task Main()
        {
            Console.Clear();
            var client = new ApiClient("https://hacker-news.firebaseio.com/v0/");
            await client.RefreshTopStoryIdsAsync();
            var stories = await client.FetchTopStoriesAsync(0, 20);
            for (var i = 0; i < stories.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write((i + 1).ToString().PadLeft(2) + ": ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(stories[i].Title);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" by " + stories[i].Creator + Environment.NewLine);
            }

            Console.ResetColor();
        }
    }
}
