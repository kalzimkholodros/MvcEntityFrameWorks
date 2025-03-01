using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace EntityMvc.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Fiyat")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Stok Miktarı")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
        public int StockQuantity { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Ürün Görseli")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
} 