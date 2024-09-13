namespace TreesNodes.ViewModels
{
    public class MJournal
    {

        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Text { get; set; }
    }
}
