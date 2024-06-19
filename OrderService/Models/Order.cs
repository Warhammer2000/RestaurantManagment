namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? Details { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
