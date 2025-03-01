namespace EntityMvc.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public ApplicationUser? User { get; set; }
    }
} 