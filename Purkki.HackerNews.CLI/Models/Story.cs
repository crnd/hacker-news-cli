using Purkki.HackerNews.CLI.DTOs;
using System;

namespace Purkki.HackerNews.CLI.Models
{
    public class Story
    {
        public Story(StoryDTO dto)
        {
            Id = dto.Id;
            Created = DateTimeOffset.FromUnixTimeSeconds(dto.Time).ToLocalTime();
            Title = dto.Title;
            Creator = dto.By;
        }

        public long Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
    }
}
