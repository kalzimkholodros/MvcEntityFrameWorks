using System.ComponentModel.DataAnnotations;

namespace EntityMvc.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sipariş")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        [Display(Name = "Miktar")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Birim Fiyat")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Toplam Tutar")]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
} 