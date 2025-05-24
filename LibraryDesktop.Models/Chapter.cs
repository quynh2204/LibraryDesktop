using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }
        
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public int ChapterNumber { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string ChapterTitle { get; set; } = string.Empty;
        
        /// <summary>
        /// GitHub raw link to the chapter content
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string GitHubContentUrl { get; set; } = string.Empty;
        
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; } = null!;
    }
}
