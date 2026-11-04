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
public class StockController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public StockController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> ListarItens()
    {
        var itens = await _context.ItensStock
            .Include(i => i.Categoria)
            .OrderBy(i => i.Nome)
            .ToListAsync();
        return Ok(itens);
    }

    [HttpGet("alertas-minimo")]
    public async Task<IActionResult> ListarAlertasMinimo()
    {
        var itens = await _context.ItensStock
            .Where(i => i.QuantidadeAtual <= i.QuantidadeMinima)
            .ToListAsync();
        return Ok(itens);
    }

    [HttpPost]
    public async Task<IActionResult> CriarItem([FromBody] ItemStockDto dto)
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não especificado.");

        var item = new ItemStock
        {
            EstabelecimentoId = _tenantService.EstabelecimentoId.Value,
            Nome = dto.Nome,
            CategoriaId = dto.CategoriaId,
            CodigoBarras = dto.CodigoBarras,
            UnidadeMedida = dto.UnidadeMedida,
            QuantidadeAtual = dto.QuantidadeAtual,
            QuantidadeMinima = dto.QuantidadeMinima,
            CustoUnitario = dto.CustoUnitario,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ItensStock.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarItem(Guid id, [FromBody] ItemStockDto dto)
    {
        var item = await _context.ItensStock.FindAsync(id);
        if (item == null) return NotFound();

        item.Nome = dto.Nome;
        item.CategoriaId = dto.CategoriaId;
        item.CodigoBarras = dto.CodigoBarras;
        item.UnidadeMedida = dto.UnidadeMedida;
        item.QuantidadeAtual = dto.QuantidadeAtual;
        item.QuantidadeMinima = dto.QuantidadeMinima;
        item.CustoUnitario = dto.CustoUnitario;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirItem(Guid id)
    {
        var item = await _context.ItensStock.FindAsync(id);
        if (item == null) return NotFound();

        _context.ItensStock.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class FornecedoresController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public FornecedoresController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Fornecedores.OrderBy(f => f.Nome).ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] FornecedorDto dto)
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não identificado.");

        var forn = new Fornecedor
        {
            EstabelecimentoId = _tenantService.EstabelecimentoId.Value,
            Nome = dto.Nome,
            Nif = dto.Nif,
            Telefone = dto.Telefone,
            Email = dto.Email,
            ContactoResponsavel = dto.ContactoResponsavel,
            Morada = dto.Morada
        };

        _context.Fornecedores.Add(forn);
        await _context.SaveChangesAsync();
        return Ok(forn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] FornecedorDto dto)
    {
        var forn = await _context.Fornecedores.FindAsync(id);
        if (forn == null) return NotFound();

        forn.Nome = dto.Nome;
        forn.Nif = dto.Nif;
        forn.Telefone = dto.Telefone;
        forn.Email = dto.Email;
        forn.ContactoResponsavel = dto.ContactoResponsavel;
        forn.Morada = dto.Morada;

        await _context.SaveChangesAsync();
        return Ok(forn);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var forn = await _context.Fornecedores.FindAsync(id);
        if (forn == null) return NotFound();

        _context.Fornecedores.Remove(forn);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public ComprasController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Compras
            .Include(c => c.Fornecedor)
            .Include(c => c.Itens)
                .ThenInclude(i => i.ItemStock)
            .OrderByDescending(c => c.DataCompra)
            .ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> RegistarCompra([FromBody] CompraCreateDto dto)
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não identificado.");

        var compra = new Compra
        {
            EstabelecimentoId = _tenantService.EstabelecimentoId.Value,
            FornecedorId = dto.FornecedorId,
            NumeroFatura = dto.NumeroFatura,
            Observacoes = dto.Observacoes,
            DataCompra = DateTime.UtcNow,
            Status = "CONCLUIDA"
        };

        decimal total = 0;
        foreach (var itemDto in dto.Itens)
        {
            var itemStock = await _context.ItensStock.FindAsync(itemDto.ItemStockId);
            if (itemStock != null)
            {
                var subtotal = itemDto.Quantidade * itemDto.PrecoUnitario;
                total += subtotal;

                compra.Itens.Add(new ItemCompra
                {
                    ItemStockId = itemDto.ItemStockId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = itemDto.PrecoUnitario,
                    Subtotal = subtotal
                });

                // ATUALIZA O STOCK AUTOMATICAMENTE COM A COMPRA
                itemStock.QuantidadeAtual += itemDto.Quantidade;
                itemStock.CustoUnitario = itemDto.PrecoUnitario; // atualiza último custo
                itemStock.UpdatedAt = DateTime.UtcNow;
            }
        }

        compra.ValorTotal = total;
        _context.Compras.Add(compra);
        await _context.SaveChangesAsync();

        return Ok(compra);
    }
}

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public ClientesController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ClienteDto dto)
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não identificado.");

        var cli = new Cliente
        {
            EstabelecimentoId = _tenantService.EstabelecimentoId.Value,
            Nome = dto.Nome,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Nif = dto.Nif,
            Morada = dto.Morada,
            Observacoes = dto.Observacoes
        };

        _context.Clientes.Add(cli);
        await _context.SaveChangesAsync();
        return Ok(cli);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] ClienteDto dto)
    {
        var cli = await _context.Clientes.FindAsync(id);
        if (cli == null) return NotFound();

        cli.Nome = dto.Nome;
        cli.Telefone = dto.Telefone;
        cli.Email = dto.Email;
        cli.Nif = dto.Nif;
        cli.Morada = dto.Morada;
        cli.Observacoes = dto.Observacoes;

        await _context.SaveChangesAsync();
        return Ok(cli);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var cli = await _context.Clientes.FindAsync(id);
        if (cli == null) return NotFound();

        _context.Clientes.Remove(cli);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class UtilizadoresController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public UtilizadoresController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Utilizadores
            .Select(u => new { u.Id, u.Nome, u.Email, u.Perfil, u.Ativo, u.CreatedAt })
            .OrderBy(u => u.Nome)
            .ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] UtilizadorDto dto)
    {
        if (!_tenantService.EstabelecimentoId.HasValue)
            return BadRequest("Tenant não identificado.");

        if (string.IsNullOrEmpty(dto.Senha))
            return BadRequest("Senha é obrigatória.");

        var user = new Utilizador
        {
            EstabelecimentoId = _tenantService.EstabelecimentoId.Value,
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = dto.Perfil,
            Ativo = dto.Ativo
        };

        _context.Utilizadores.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Nome, user.Email, user.Perfil, user.Ativo });
    }
}
