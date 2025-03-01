using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EntityMvc.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Kayıt Tarihi")]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        [Display(Name = "Ad Soyad")]
        public string FullName => $"{FirstName} {LastName}";
    }
} 