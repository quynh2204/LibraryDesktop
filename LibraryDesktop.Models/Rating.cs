using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int BookId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }
        
        [MaxLength(500)]
        public string? Review { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        
        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; } = null!;
    }
}
