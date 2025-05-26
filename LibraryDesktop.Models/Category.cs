using System.ComponentModel.DataAnnotations;

namespace LibraryDesktop.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
