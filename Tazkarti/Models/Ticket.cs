using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }
        public Match Match { get; set; }
    }
}