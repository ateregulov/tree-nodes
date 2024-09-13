using System.ComponentModel.DataAnnotations;

namespace TreesNodes.DAL
{
    public class JournalItem
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Query { get; set; }
        public string Body { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
