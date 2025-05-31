using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public enum ThemeMode
    {
        Light = 0,
        Dark = 1,
        Auto = 2
    }

    public class UserSetting
    {
        [Key]
        public int SettingId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public ThemeMode ThemeMode { get; set; } = ThemeMode.Light;
          [Required]
        [Range(8, 72)]
        public int FontSize { get; set; } = 12;
        
        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
