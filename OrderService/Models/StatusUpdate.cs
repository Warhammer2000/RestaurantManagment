namespace OrderService.Models
{
    public class StatusUpdate
    {
        public int OrderId { get; set; }
        public string? NewStatus { get; set; }
    }
}
