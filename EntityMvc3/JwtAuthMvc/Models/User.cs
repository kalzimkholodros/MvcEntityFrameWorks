using System.ComponentModel.DataAnnotations;

namespace JwtAuthMvc.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir")]
        public string Password { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        public string? Role { get; set; }

        [Display(Name = "Bakiye")]
        public decimal Balance { get; set; } = 0;

        [Display(Name = "Premium Üye")]
        public bool IsPremium { get; set; } = false;

        [Display(Name = "Premium Bitiş Tarihi")]
        public DateTime? PremiumEndDate { get; set; }
    }
} 