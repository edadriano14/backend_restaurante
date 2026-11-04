using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public SalasController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sala>>> GetAll()
    {
        return await _context.Salas.Include(s => s.Mesas).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Sala>> Create(Sala model)
    {
        if (_tenantService.EstabelecimentoId.HasValue)
        {
            model.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        }
        _context.Salas.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
    }
}

[ApiController]
[Route("api/[controller]")]
public class MesasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public MesasController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mesa>>> GetAll()
    {
        return await _context.Mesas.Include(m => m.Sala).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Mesa>> Create(Mesa model)
    {
        if (_tenantService.EstabelecimentoId.HasValue)
        {
            model.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        }
        if (string.IsNullOrEmpty(model.QrCodeToken))
        {
            model.QrCodeToken = "QR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
        _context.Mesas.Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
    }

    [HttpPut("{id}/estado")]
    public async Task<IActionResult> UpdateEstado(Guid id, [FromBody] string novoEstado)
    {
        var mesa = await _context.Mesas.FindAsync(id);
        if (mesa == null) return NotFound();
        mesa.Estado = novoEstado;
        await _context.SaveChangesAsync();
        return Ok(mesa);
    }
}
