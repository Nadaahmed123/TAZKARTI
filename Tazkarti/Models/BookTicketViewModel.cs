namespace Tazkarti.Models
{
    public class BookTicketViewModel
    {
        public int MatchId { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }
        public Match Match { get; set; } 

    }
}
