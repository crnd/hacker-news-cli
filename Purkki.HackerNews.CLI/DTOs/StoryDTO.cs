using System.Collections.Generic;

namespace Purkki.HackerNews.CLI.DTOs
{
    public class StoryDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long Time { get; set; }
        public string By { get; set; }
        public IList<long> Kids { get; set; }
    }
}
