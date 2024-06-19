using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderManagement.DB;
using RestaurantOrderManagement.Models;
using RestaurantOrderManagement.Services;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

[Route("[controller]")]
public class MenuController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RabbitMQService _rabbitMQService;

    public MenuController(IHttpClientFactory httpClientFactory, RabbitMQService rabbitMQService)
    {
        _httpClientFactory = httpClientFactory;
        _rabbitMQService = rabbitMQService;
    }

    // GET: /Menu
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("MenuService");
        var response = await client.GetAsync("api/menuitems");

        if (response.IsSuccessStatusCode)
        {
            var menuItems = JsonSerializer.Deserialize<List<MenuItem>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(menuItems);
        }

        return View(new List<MenuItem>());
    }


    // GET: /Menu/Create
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Menu/Create
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Price")] MenuItem newItem)
    {
        var client = _httpClientFactory.CreateClient("MenuService");
        var jsonContent = JsonSerializer.Serialize(newItem);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/menuitems", content);
        response.EnsureSuccessStatusCode();
        _rabbitMQService.SendMessage(jsonContent, "menu");
        return RedirectToAction(nameof(Index));
    }

    // GET: /Menu/Edit/5
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var client = _httpClientFactory.CreateClient("MenuService");
        var response = await client.GetAsync($"api/menuitems/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var menuItem = JsonSerializer.Deserialize<MenuItem>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(menuItem);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] MenuItem updatedItem)
    {
        if (id != updatedItem.Id)
        {
            return BadRequest("Menu item ID mismatch.");
        }

        var client = _httpClientFactory.CreateClient("MenuService");
        var jsonContent = JsonSerializer.Serialize(updatedItem);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"api/menuitems/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorMessage);
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _httpClientFactory.CreateClient("MenuService");
        var response = await client.GetAsync($"api/menuitems/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Menu item not found.");
        }

        var menuItemJson = await response.Content.ReadAsStringAsync();
        var menuItem = JsonSerializer.Deserialize<MenuItem>(menuItemJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (menuItem == null)
        {
            return NotFound("Menu item data is null.");
        }

        return View(menuItem);
    }


    [HttpPost("Delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _httpClientFactory.CreateClient("MenuService");
        var response = await client.DeleteAsync($"api/menuitems/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorMessage);
        }

        return RedirectToAction(nameof(Index));
    }
}