using System;
using System.Collections.Generic;

namespace Purkki.HackerNews.CLI.Models
{
    public class Story
    {
        public long Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
        public IList<long> CommentIds { get; set; }
        public int CommentCount => CommentIds?.Count ?? 0;
    }
}
