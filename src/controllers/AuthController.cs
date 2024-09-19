using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YourProjectNamespace.Models; // Importa o modelo User
using YourProjectNamespace.Data; // Importa o DbContext
using Microsoft.EntityFrameworkCore;

[Route("api/[controller")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context)
    {
        _context = context;
        _configuration = Configuration;
    }

    //Registrar usuário
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        var user = new User
        {
            Usuario = userDto.Usuario,
            Email = userDto.Email,
            Senha = BCrypt.Net.BCrypt.HashPassword(userDto.Senha) // Hash da senha
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Usuário registrado com sucesso!");
    }

    // Método de Login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Usuario == userDto.Usuario);

        if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Senha, user.Senha))
        {
            return Unauthorized("Usuário ou senha incorretos");
        }

        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    // Método para gerar o JWT
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Usuario)
            }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiresIn"])),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}