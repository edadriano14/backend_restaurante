using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.DTOs;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // Ignora query filter no login para encontrar o utilizador pelo email em qualquer tenant
        var user = await _context.Utilizadores
            .IgnoreQueryFilters()
            .Include(u => u.Estabelecimento)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == req.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Senha, user.SenhaHash))
        {
            return Unauthorized(new { message = "Email ou senha incorretos." });
        }

        if (!user.Ativo)
        {
            return Unauthorized(new { message = "Utilizador desativado." });
        }

        var est = user.Estabelecimento;
        var token = TokenService.GenerateJwt(user, _config);

        return Ok(new LoginResponse(
            Token: token,
            UtilizadorId: user.Id,
            Nome: user.Nome,
            Email: user.Email,
            Perfil: user.Perfil,
            EstabelecimentoId: user.EstabelecimentoId,
            EstabelecimentoNome: est?.Nome ?? "Restaurante",
            CorPrimaria: est?.CorPrimaria ?? "#E53935",
            CorSecundaria: est?.CorSecundaria ?? "#1E293B",
            LogotipoUrl: est?.LogotipoUrl
        ));
    }
}
