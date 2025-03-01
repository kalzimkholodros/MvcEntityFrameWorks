using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthMvc.Models
{
    public class Premium
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [Display(Name = "Kayıt Tarihi")]
        public DateTime SubscriptionDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Ödenen Ücret")]
        public decimal PaidAmount { get; set; }

        [Display(Name = "Aktif Mi")]
        public bool IsActive { get; set; } = true;

        // Otomatik olarak hesaplanacak kalan gün sayısı
        [NotMapped]
        public int RemainingDays => (ExpiryDate - DateTime.Now).Days;
    }
} 