using Newtonsoft.Json;
using Purkki.HackerNews.CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Purkki.HackerNews.CLI
{
    public class ApiClient
    {
        private readonly string _endpoint;
        private readonly HttpClient _client;
        private IList<long> _topStoryIds;
        private IDictionary<long, Story> _topStories;
        private IDictionary<long, Comment> _comments;

        public int StoryCount => _topStoryIds?.Count ?? 0;

        public ApiClient(string endpoint)
        {
            _endpoint = endpoint;
            _client = new HttpClient();
            _topStories = new Dictionary<long, Story>();
            _comments = new Dictionary<long, Comment>();
        }

        public async Task RefreshTopStoryIdsAsync()
        {
            var storiesRequest = new HttpRequestMessage(HttpMethod.Get, _endpoint + "topstories.json");
            var response = await _client.SendAsync(storiesRequest);
            var json = await response.Content.ReadAsStringAsync();
            _topStoryIds = JsonConvert.DeserializeObject<List<long>>(json);
            _topStories.Clear();
            _comments.Clear();
        }

        public async Task<IList<Story>> FetchTopStoriesAsync(int skip, int count)
        {
            var stories = new List<Story>();
            foreach (var id in _topStoryIds.Skip(skip).Take(count))
            {
                if (_topStories.ContainsKey(id))
                {
                    stories.Add(_topStories[id]);
                }
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + $"item/{id}.json");
                    var response = await _client.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<StoryDTO>(json);
                    var story = new Story
                    {
                        Id = dto.Id,
                        Created = DateTimeOffset.FromUnixTimeSeconds(dto.Time).ToLocalTime(),
                        Title = dto.Title,
                        Creator = dto.By,
                        CommentIds = dto.Kids
                    };
                    stories.Add(story);
                    _topStories.Add(id, story);
                }
            }

            return stories;
        }

        public async Task<IList<Comment>> FetchStoryCommentsAsync(long storyId)
        {
            var comments = new List<Comment>();
            foreach (var commentId in _topStories[storyId].CommentIds)
            {
                comments.Add(await FetchCommentAsync(commentId));
            }

            return comments;
        }

        private async Task<Comment> FetchCommentAsync(long commentId)
        {
            Comment comment;
            if (_comments.ContainsKey(commentId))
            {
                comment = _comments[commentId];
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _endpoint + $"item/{commentId}.json");
                var response = await _client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<CommentDTO>(json);
                comment = new Comment
                {
                    Id = dto.Id,
                    Text = dto.Text,
                    Created = DateTimeOffset.FromUnixTimeSeconds(dto.Time).ToLocalTime(),
                    Creator = dto.By,
                    CommentIds = dto.Kids
                };
                _comments.Add(commentId, comment);
            }

            if (comment.CommentIds != null)
            {
                comment.Comments = new List<Comment>();
                foreach (var id in comment.CommentIds)
                {
                    comment.Comments.Add(await FetchCommentAsync(id));
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
