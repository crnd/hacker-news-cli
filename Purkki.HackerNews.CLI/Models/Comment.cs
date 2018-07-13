using System;
using System.Collections.Generic;

namespace Purkki.HackerNews.CLI.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Creator { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<long> CommentIds { get; set; }
    }
}
