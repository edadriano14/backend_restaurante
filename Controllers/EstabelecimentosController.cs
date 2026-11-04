using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.DTOs;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstabelecimentosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public EstabelecimentosController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> ListarTodos()
    {
        var lista = await _context.Estabelecimentos
            .OrderBy(e => e.Nome)
            .ToListAsync();
        return Ok(lista);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentEstabelecimento()
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não especificado.");

        var est = await _context.Estabelecimentos.FindAsync(_tenantService.EstabelecimentoId.Value);
        if (est == null) return NotFound("Estabelecimento não encontrado.");

        return Ok(est);
    }

    [HttpPost]
    public async Task<IActionResult> CriarNovoEstabelecimento([FromBody] Estabelecimento est)
    {
        if (string.IsNullOrWhiteSpace(est.Nome))
            return BadRequest("O nome do estabelecimento é obrigatório.");

        if (string.IsNullOrWhiteSpace(est.Slug))
        {
            est.Slug = est.Nome.ToLower()
                .Replace(" ", "-")
                .Replace("&", "e")
                .Replace("ç", "c")
                .Replace("ã", "a")
                .Replace("õ", "o")
                .Replace("é", "e")
                .Replace("á", "a") + "-" + DateTime.UtcNow.Ticks.ToString().Substring(12);
        }

        est.Id = Guid.NewGuid();
        est.CreatedAt = DateTime.UtcNow;
        est.UpdatedAt = DateTime.UtcNow;

        _context.Estabelecimentos.Add(est);
        await _context.SaveChangesAsync();

        return Ok(est);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentEstabelecimento([FromBody] EstabelecimentoUpdateRequest req)
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não especificado.");

        var est = await _context.Estabelecimentos.FindAsync(_tenantService.EstabelecimentoId.Value);
        if (est == null) return NotFound("Estabelecimento não encontrado.");

        est.Nome = req.Nome;
        est.LogotipoUrl = req.LogotipoUrl;
        est.CorPrimaria = req.CorPrimaria;
        est.CorSecundaria = req.CorSecundaria;
        est.Telefone = req.Telefone;
        est.Email = req.Email;
        est.Endereco = req.Endereco;
        if (!string.IsNullOrEmpty(req.Moeda)) est.Moeda = req.Moeda;
        est.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(est);
    }
}
