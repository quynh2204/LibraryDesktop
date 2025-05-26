using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public class UserFavorite
    {
        [Key]
        public int FavoriteId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int BookId { get; set; }
        
        public DateTime AddedDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        
        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; } = null!;
    }
}
