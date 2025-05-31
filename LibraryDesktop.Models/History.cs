using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int BookId { get; set; }
        
        public int? ChapterId { get; set; }
        
        [Required]
        public DateTime AccessedDate { get; set; } = DateTime.Now;
        
        [MaxLength(50)]
        public string AccessType { get; set; } = "View"; // "View", "Read", "Chapter"
        
        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        
        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; } = null!;
        
        [ForeignKey(nameof(ChapterId))]
        public virtual Chapter? Chapter { get; set; }
    }
}
