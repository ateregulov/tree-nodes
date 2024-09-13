using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace TreesNodes.DAL
{
    [Index(nameof(Name))]
    public class Node
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public Node? Parent { get; set; } 
        public List<Node>? Children { get; set; }
    }
}
