using Purkki.HackerNews.CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purkki.HackerNews.CLI
{
	class Program
	{
		static async Task Main()
		{
			Helpers.ClearConsole();
			Helpers.PrintLoadingScreen();

			var client = ApiHelper.CreateHttpClient("https://hacker-news.firebaseio.com/v0/");
			var storyIds = await ApiHelper.GetTopStoryIdsAsync(client);
			var stories = new Dictionary<long, Story>();

			var key = ConsoleKey.NoName;
			var index = 0;
			bool render = true;

			while (key != ConsoleKey.Q)
			{
				if (render)
				{
					var visibleToStoryIds = storyIds.Skip(index).Take(Console.WindowHeight - 1).ToList();
					var visibleStories = await ApiHelper.GetTopStoriesAsync(client, visibleToStoryIds, stories);
					Helpers.ClearConsole();
					Helpers.PrintStories(visibleStories);
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
						if (index != storyIds.Count - 1)
						{
							index++;
							render = true;
						}
						break;
					case ConsoleKey.R:
						Helpers.ClearConsole();
						Helpers.PrintLoadingScreen();
						storyIds = await ApiHelper.GetTopStoryIdsAsync(client);
						stories.Clear();
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
