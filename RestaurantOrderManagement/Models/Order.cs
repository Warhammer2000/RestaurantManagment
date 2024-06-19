using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderManagement.Models
{
	public class Order
	{
		[Key]
		public int Id { get; set; }
		public string? Details { get; set; }
		public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
