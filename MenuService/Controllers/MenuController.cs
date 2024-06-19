using MenuService.DB;
using MenuService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MenuService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly MenuContext _context;
        public MenuItemsController(MenuContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return Ok(menuItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return Ok(menuItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItem newItem)
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			newItem.Price = AdjustPrice(newItem.Price);

			await _context.MenuItems.AddAsync(newItem);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMenuItem), new { id = newItem.Id }, newItem);
		}
		private decimal AdjustPrice(decimal price)
		{
			return price;
		}
		[HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItem updatedItem)
        {
			if (id != updatedItem.Id)
			{
				return BadRequest("ID mismatch.");
			}

			updatedItem.Price = AdjustPrice(updatedItem.Price);

			_context.Entry(updatedItem).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.MenuItems.Any(mi => mi.Id == id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
