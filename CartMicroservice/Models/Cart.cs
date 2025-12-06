namespace CartMicroservice.Models
{
    public class Cart
    {
        public string UserId { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}