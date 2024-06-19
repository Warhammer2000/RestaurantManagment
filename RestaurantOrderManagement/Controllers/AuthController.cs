using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestaurantOrderManagement.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("[controller]")]
[ApiController]
public class AuthController : Controller
{
	[HttpPost("Login")]
	public IActionResult Login([FromBody] UserLoginModel loginModel)
	{
		if (loginModel.Username == "admin" && loginModel.Password == "password")
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("yourSecretKey");
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, loginModel.Username) }),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("", "Invalid login attempt.");
        return View(loginModel); // Возвращаемся к представлению с ошибками
    }
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }
}