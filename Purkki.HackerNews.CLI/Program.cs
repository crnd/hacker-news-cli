using System;
using System.Threading.Tasks;

namespace Purkki.HackerNews.CLI
{
    class Program
    {
        static async Task Main()
        {
            Helpers.ClearConsole();
            Helpers.PrintLoadingScreen();

            var client = new ApiClient("https://hacker-news.firebaseio.com/v0/");
            await client.RefreshTopStoryIdsAsync();

            var key = ConsoleKey.NoName;
            var index = 0;
            bool render = true;

            while (key != ConsoleKey.Q)
            {
                if (render)
                {
                    var stories = await client.FetchTopStoriesAsync(index, Console.WindowHeight - 1);
                    Helpers.ClearConsole();
                    Helpers.PrintStories(stories);
                    render = false;
                }

                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (index != 0)
                        {
                            index--;
                            render = true;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (index != client.StoryCount - 1)
                        {
                            index++;
                            render = true;
                        }
                        break;
                    case ConsoleKey.R:
                        Helpers.ClearConsole();
                        Helpers.PrintLoadingScreen();
                        await client.RefreshTopStoryIdsAsync();
                        index = 0;
                        render = true;
                        break;
                    default:
                        break;
                }
            }

            Helpers.ClearConsole();
        }
    }
}
