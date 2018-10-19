using Newtonsoft.Json;
using Purkki.HackerNews.CLI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Purkki.HackerNews.CLI
{
	public static class ApiHelper
	{
		public static HttpClient CreateHttpClient(string endpoint)
		{
			if (string.IsNullOrWhiteSpace(endpoint))
			{
				throw new ArgumentNullException(nameof(endpoint));
			}

			return new HttpClient
			{
				BaseAddress = new Uri(endpoint)
			};
		}

		public static async Task<IList<long>> GetTopStoryIdsAsync(HttpClient client)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			var response = await client.GetAsync("topstories.json");
			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<long>>(json);
		}

		public static async Task<IList<Story>> GetTopStoriesAsync(HttpClient client, IList<long> ids, IDictionary<long, Story> stories)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (ids == null)
			{
				throw new ArgumentNullException(nameof(ids));
			}

			if (stories == null)
			{
				throw new ArgumentNullException(nameof(stories));
			}

			var visibleStories = new List<Story>();
			foreach (var id in ids)
			{
				if (!stories.ContainsKey(id))
				{
					var response = await client.GetAsync($"item/{id}.json");
					var json = await response.Content.ReadAsStringAsync();
					var dto = JsonConvert.DeserializeObject<StoryDTO>(json);
					stories.Add(id, new Story
					{
						Id = id,
						Created = DateTimeOffset.FromUnixTimeSeconds(dto.Time).ToLocalTime(),
						Title = dto.Title,
						Creator = dto.By,
						CommentIds = dto.Kids
					});
				}

				visibleStories.Add(stories[id]);
			}

			return visibleStories;
		}

		public static async Task<IList<Comment>> GetCommentsAsync(HttpClient client, IList<long> commentIds)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (commentIds == null)
			{
				throw new ArgumentNullException(nameof(commentIds));
			}

			var comments = new List<Comment>();
			foreach (var commentId in commentIds)
			{
				comments.Add(await GetCommentAsync(client, commentId));
			}

			return comments;
		}

		private static async Task<Comment> GetCommentAsync(HttpClient client, long commentId)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			var response = await client.GetAsync($"item/{commentId}.json");
			var json = await response.Content.ReadAsStringAsync();
			var dto = JsonConvert.DeserializeObject<CommentDTO>(json);
			var comment = new Comment
			{
				Id = dto.Id,
				Text = dto.Text,
				Created = DateTimeOffset.FromUnixTimeSeconds(dto.Time).ToLocalTime(),
				Creator = dto.By,
				CommentIds = dto.Kids
			};

			if (comment.CommentIds != null)
			{
				comment.Comments = new List<Comment>();
				foreach (var id in comment.CommentIds)
				{
					comment.Comments.Add(await GetCommentAsync(client, id));
				}
			}

			return comment;
		}

		private class StoryDTO
		{
			public long Id { get; set; }
			public string Title { get; set; }
			public long Time { get; set; }
			public string By { get; set; }
			public IList<long> Kids { get; set; }
		}

		private class CommentDTO
		{
			public long Id { get; set; }
			public long Time { get; set; }
			public string Text { get; set; }
			public string By { get; set; }
			public IList<long> Kids { get; set; }
		}
	}
}
