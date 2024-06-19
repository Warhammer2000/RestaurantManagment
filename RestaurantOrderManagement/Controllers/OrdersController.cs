using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderManagement.DB;
using RestaurantOrderManagement.Models;
using RestaurantOrderManagement.Services;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RabbitMQService _rabbitMQService;
    public OrdersController(IHttpClientFactory httpClientFactory, RabbitMQService rabbitMQService)
    {
        _httpClientFactory = httpClientFactory;
        _rabbitMQService = rabbitMQService;
    }

    // GET: /Orders
    [HttpGet]
    public async Task<IActionResult> Index(string status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var client = _httpClientFactory.CreateClient("OrderService");

        // Построение строки запроса с параметрами фильтрации
        var query = new List<string>();
        if (!string.IsNullOrEmpty(status))
        {
            query.Add($"status={status}");
        }
        if (startDate.HasValue)
        {
            query.Add($"startDate={startDate.Value.ToString("yyyy-MM-dd")}");
        }
        if (endDate.HasValue)
        {
            query.Add($"endDate={endDate.Value.ToString("yyyy-MM-dd")}");
        }
        var queryString = string.Join("&", query);

        var response = await client.GetAsync($"api/orders?{queryString}");

        if (response.IsSuccessStatusCode)
        {
            var ordersJson = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(orders);
        }

        return View(new List<Order>());
    }

    // GET: /Orders/Details/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var client = _httpClientFactory.CreateClient("OrderService");

        var response = await client.GetAsync($"api/orders/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Order not found.");
        }
        var orderJson = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<Order>(orderJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (order == null)
        {
            return NotFound("Order data is null.");
        }

        return View(order);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Orders/Create
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Order newOrder)
    {
        var client = _httpClientFactory.CreateClient("OrderService");

        var jsonContent = JsonSerializer.Serialize(newOrder);

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/orders", content);

        if (response.IsSuccessStatusCode)
        {
            _rabbitMQService.SendMessage(jsonContent, "order"); 
            return RedirectToAction(nameof(Index));
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorMessage);
        }
    }
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var client = _httpClientFactory.CreateClient("OrderService");
        var response = await client.GetAsync($"api/orders/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Order not found.");
        }

        var orderJson = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<Order>(orderJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (order == null)
        {
            return NotFound("Order data is null.");
        }

        return View(order);
    }


    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] Order updatedOrder)
    {
        if (id != updatedOrder.Id)
        {
            return BadRequest("Order ID mismatch.");
        }

        var client = _httpClientFactory.CreateClient("OrderService");
        var jsonContent = JsonSerializer.Serialize(updatedOrder);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"api/orders/{id}", content);
        var message = $"Order {id} updated to status {updatedOrder.Status}";
        _rabbitMQService.SendMessage(message, "status");
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
        var client = _httpClientFactory.CreateClient("OrderService");
        var response = await client.GetAsync($"api/orders/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("Order not found.");
        }

        // Десериализация JSON ответа в объект Order
        var orderJson = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<Order>(orderJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (order == null)
        {
            return NotFound("Order data is null.");
        }

        return View(order);
    }


    [HttpPost("Delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = _httpClientFactory.CreateClient("OrderService");
        var response = await client.DeleteAsync($"api/orders/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorMessage);
        }

        return RedirectToAction(nameof(Index));
    }
}
