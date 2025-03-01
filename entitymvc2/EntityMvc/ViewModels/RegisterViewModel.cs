using System.ComponentModel.DataAnnotations;

namespace EntityMvc.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam ve ._- karakterlerini içerebilir")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 