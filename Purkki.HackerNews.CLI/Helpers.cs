using Purkki.HackerNews.CLI.Models;
using System;
using System.Collections.Generic;

namespace Purkki.HackerNews.CLI
{
    public static class Helpers
    {
        public static void ClearConsole()
        {
            Console.ResetColor();
            Console.Clear();
        }

        public static void PrintLoadingScreen()
        {
            Console.SetCursorPosition((Console.WindowWidth - Constants.FetchingStories.Length) / 2, Console.WindowHeight / 2);
            Console.Write(Constants.FetchingStories);
        }

        public static void PrintStories(IList<Story> stories)
        {
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
