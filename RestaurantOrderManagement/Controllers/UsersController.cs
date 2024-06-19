using Microsoft.AspNetCore.Mvc;
using RestaurantOrderManagement.DB;
using RestaurantOrderManagement.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
	private readonly AppDbContext _context;

	public UsersController(AppDbContext context)
	{
		_context = context;
	}


    [HttpGet]
    public IActionResult Index()
    {
        return View(); // Возвращает представление Index
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(); // Возврат представления для регистрации
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User newUser)
    {
        if (_context.Users.Any(u => u.Username == newUser.Username))
        {
            ModelState.AddModelError("Username", "Username already exists.");
            return View(newUser); // Возврат представления с ошибкой
        }

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return RedirectToAction("Login"); // Перенаправление на Login после успешной регистрации
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View(); // Возврат представления для входа
    }

    [HttpPost("login")]
    public IActionResult Login(UserLoginModel loginModel)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == loginModel.Username && u.Password == loginModel.Password);
        if (user == null)
        {
            ModelState.AddModelError("LoginError", "Invalid username or password.");
            return View(loginModel); // Возврат представления с ошибкой
        }

        // Генерация JWT токена будет добавлена позже
        return RedirectToAction("Index", "Home"); // Перенаправление на домашнюю страницу после успешного входа
    }
}