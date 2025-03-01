using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityMvc.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sipariş Numarası")]
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        [Required]
        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Display(Name = "Sipariş Tarihi")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Sipariş Durumu")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Toplam Tutar")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Teslimat Adresi")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Display(Name = "Notlar")]
        public string Notes { get; set; } = string.Empty;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        [Display(Name = "Beklemede")]
        Pending,
        
        [Display(Name = "Onaylandı")]
        Confirmed,
        
        [Display(Name = "Kargoda")]
        Shipped,
        
        [Display(Name = "Tamamlandı")]
        Completed,
        
        [Display(Name = "İptal Edildi")]
        Cancelled
    }
} 