using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace EntityMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public decimal Balance { get; set; } = 100; // Başlangıç bakiyesi
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
} 