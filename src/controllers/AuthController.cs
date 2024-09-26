using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GerenciaAPI.Models; // Usando Models conforme especificado
using Microsoft.EntityFrameworkCore;
using System;

namespace GerenciaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Registrar usu�rio
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var usuario = new Usuario
            {
                Nome = userDto.Usuario,
                Email = userDto.Email,
                Telefone = "", // Adapte se necess�rio
            };
            usuario.DefinirSenha(userDto.Senha); // Definindo o hash da senha

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usu�rio registrado com sucesso!");
        }

        // M�todo de Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nome == loginDto.Usuario);

            if (usuario == null || !usuario.VerificarSenha(loginDto.Senha))
            {
                return Unauthorized("Usu�rio ou senha incorretos");
            }

            var token = GenerateJwtToken(usuario);

            return Ok(new { Token = token });
        }

        // M�todo para gerar o JWT
        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome)
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
}