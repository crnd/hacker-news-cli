using Newtonsoft.Json;
using Purkki.HackerNews.CLI.DTOs;
using Purkki.HackerNews.CLI.Models;
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

        public int StoryCount => _topStoryIds?.Count ?? 0;

        public ApiClient(string endpoint)
        {
            _endpoint = endpoint;
            _client = new HttpClient();
            _topStories = new Dictionary<long, Story>();
        }

        public async Task RefreshTopStoryIdsAsync()
        {
            var storiesRequest = new HttpRequestMessage(HttpMethod.Get, _endpoint + "topstories.json");
            var response = await _client.SendAsync(storiesRequest);
            var json = await response.Content.ReadAsStringAsync();
            _topStoryIds = JsonConvert.DeserializeObject<List<long>>(json);
            _topStories.Clear();
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
                    var story = new Story(dto);
                    stories.Add(story);
                    _topStories.Add(id, story);
                }
            }

            return stories;
        }
    }
}
