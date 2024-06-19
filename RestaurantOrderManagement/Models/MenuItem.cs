﻿using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderManagement.Models
{
	public class MenuItem
	{
		[Key]
		public int Id { get; set; }
		public string? Name { get; set; }
		public decimal? Price { get; set; }
	}
}
