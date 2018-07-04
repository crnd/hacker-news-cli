using Purkki.HackerNews.CLI.Models;
using System;
using System.Collections.Generic;
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

            var key = ConsoleKey.NoName;
            var index = 0;
            bool render = true;

            while (key != ConsoleKey.Q)
            {
                if (render)
                {
                    var stories = await client.FetchTopStoriesAsync(index, Console.WindowHeight - 1);
                    PrintStories(stories);
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
                    default:
                        break;
                }
            }

            Console.ResetColor();
            Console.Clear();
        }

        private static void PrintStories(IList<Story> stories)
        {
            Console.Clear();

            PrintStoryRow(stories[0], ConsoleColor.White, ConsoleColor.Gray);
            for (var i = 1; i < stories.Count; i++)
            {
                PrintStoryRow(stories[i], ConsoleColor.Gray, ConsoleColor.DarkGray);
            }
        }

        private static void PrintStoryRow(Story story, ConsoleColor primary, ConsoleColor secondary)
        {
            Console.ForegroundColor = primary;
            Console.Write(story.Title);
            Console.ForegroundColor = secondary;
            Console.Write($" by {story.Creator}" + Environment.NewLine);
        }
    }
}
