using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public enum BookStatus
    {
        Draft = 0,
        Published = 1,
        Completed = 2,
        OnHold = 3,
        Cancelled = 4
    }

    public class Book
    {
        [Key]
        public int BookId { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? CoverImageUrl { get; set; }
        
        public int TotalChapters { get; set; } = 0;
        
        [Required]
        public BookStatus Status { get; set; } = BookStatus.Draft;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [Required]
        public int ViewCount { get; set; }
        
        public int Price { get; set; } = 0;
        
        // Navigation properties
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;
        
        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
        public virtual ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
