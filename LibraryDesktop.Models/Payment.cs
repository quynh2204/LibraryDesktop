using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryDesktop.Models
{
    public enum PaymentMethod
    {
        QRCode = 0,
        CreditCard = 1,
        Wallet = 2
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Cancelled = 3
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.QRCode;
        
        [Required]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        
        public string? QrCodeData { get; set; }
        
        /// <summary>
        /// Unique token for QR code verification
        /// </summary>
        [MaxLength(100)]
        public string? PaymentToken { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? CompletedDate { get; set; }
        
        [MaxLength(200)]
        public string? Description { get; set; }
        
        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        public string? PaymentUrl { get; set; }
    }
}
