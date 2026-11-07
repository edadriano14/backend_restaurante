using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetodosPagamentoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public MetodosPagamentoController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetodoPagamento>>> GetAll()
    {
        return await _context.MetodosPagamento.OrderBy(m => m.Ordem).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<MetodoPagamento>> Create(MetodoPagamento model)
    {
        if (_tenantService.EstabelecimentoId.HasValue)
        {
            model.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        }
        _context.MetodosPagamento.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
    }
}

[ApiController]
[Route("api/[controller]")]
public class UnidadesMedidaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public UnidadesMedidaController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnidadeMedida>>> GetAll()
    {
        return await _context.UnidadesMedida.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<UnidadeMedida>> Create(UnidadeMedida model)
    {
        if (_tenantService.EstabelecimentoId.HasValue)
        {
            model.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        }
        _context.UnidadesMedida.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
    }
}

[ApiController]
[Route("api/[controller]")]
public class ArmazensController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public ArmazensController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Armazem>>> GetAll()
    {
        return await _context.Armazens.Include(a => a.Locais).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Armazem>> Create(Armazem model)
    {
        if (_tenantService.EstabelecimentoId.HasValue)
        {
            model.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        }
        _context.Armazens.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Armazem model)
    {
        var existing = await _context.Armazens.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Nome = model.Nome;
        existing.Codigo = model.Codigo;
        existing.Tipo = model.Tipo;
        existing.Ativo = model.Ativo;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _context.Armazens.FindAsync(id);
        if (existing == null) return NotFound();

        _context.Armazens.Remove(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
