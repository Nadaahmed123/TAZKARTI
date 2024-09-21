using System;
using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string? TeamAImage { get; set; }  
        public string? TeamBImage { get; set; }  
        public DateTime MatchDate { get; set; }
        public TimeSpan MatchTime { get; set; }
    }
}
