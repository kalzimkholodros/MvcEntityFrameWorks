using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace EntityMVC.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

        public string GravatarUrl 
        { 
            get 
            {
                using var md5 = MD5.Create();
                var hash = BitConverter.ToString(
                    md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(Email.ToLower())))
                    .Replace("-", "")
                    .ToLower();
                return $"https://www.gravatar.com/avatar/{hash}?s=200&d=mp";
            }
        }
    }
} 