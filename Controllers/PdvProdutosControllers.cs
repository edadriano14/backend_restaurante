using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public CategoriasController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Categorias
            .Include(c => c.Produtos)
            .OrderBy(c => c.Ordem)
            .ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Categoria cat)
    {
        if (!_tenantService.EstabelecimentoId.HasValue) return BadRequest("Tenant não definido.");
        
        // Auto ordem
        var maxOrdem = await _context.Categorias.MaxAsync(c => (int?)c.Ordem) ?? 0;
        cat.Ordem = maxOrdem + 1;
        cat.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        cat.CreatedAt = DateTime.UtcNow;

        _context.Categorias.Add(cat);
        await _context.SaveChangesAsync();
        return Ok(cat);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] Categoria cat)
    {
        var item = await _context.Categorias.FindAsync(id);
        if (item == null) return NotFound();

        item.Codigo = cat.Codigo;
        item.Nome = cat.Nome;
        item.Descricao = cat.Descricao;
        item.Emoji = cat.Emoji;
        item.Ordem = cat.Ordem;
        item.Ativo = cat.Ativo;

        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var item = await _context.Categorias.FindAsync(id);
        if (item == null) return NotFound();

        _context.Categorias.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public ProdutosController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? categoriaId, [FromQuery] bool? emDestaque)
    {
        var query = _context.Produtos.Include(p => p.Categoria).AsQueryable();

        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == categoriaId.Value);

        if (emDestaque.HasValue)
            query = query.Where(p => p.EmDestaque == emDestaque.Value);

        var lista = await query.OrderByDescending(p => p.TotalComprados).ThenBy(p => p.Nome).ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Produto p)
    {
        if (!_tenantService.EstabelecimentoId.HasValue) return BadRequest("Tenant não definido.");

        p.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        p.CreatedAt = DateTime.UtcNow;
        p.UpdatedAt = DateTime.UtcNow;

        _context.Produtos.Add(p);
        await _context.SaveChangesAsync();
        return Ok(p);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] Produto p)
    {
        var item = await _context.Produtos.FindAsync(id);
        if (item == null) return NotFound();

        item.CodigoProduto = p.CodigoProduto;
        item.Nome = p.Nome;
        item.Descricao = p.Descricao;
        item.CategoriaId = p.CategoriaId;
        item.Preco = p.Preco;
        item.Estado = p.Estado;
        item.ImagemUrl = p.ImagemUrl;
        item.GrupoOpcoesAcompanhamento = p.GrupoOpcoesAcompanhamento;
        item.TemPromocao = p.TemPromocao;
        item.PrecoPromocional = p.PrecoPromocional;
        item.HoraInicioPromo = p.HoraInicioPromo;
        item.HoraFimPromo = p.HoraFimPromo;
        item.DiasSemanaPromo = p.DiasSemanaPromo;
        item.EmDestaque = p.EmDestaque;
        item.ItemStockId = p.ItemStockId;
        item.ControlaStock = p.ControlaStock;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var item = await _context.Produtos.FindAsync(id);
        if (item == null) return NotFound();

        _context.Produtos.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class GuiasEntradaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public GuiasEntradaController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.GuiasEntrada
            .Include(g => g.Itens)
                .ThenInclude(i => i.ItemStock)
            .OrderByDescending(g => g.DataEmissao)
            .ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessarGuia([FromBody] GuiaEntrada guia)
    {
        if (!_tenantService.EstabelecimentoId.HasValue) return BadRequest("Tenant não definido.");

        guia.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        guia.CreatedAt = DateTime.UtcNow;
        guia.Status = "PROCESSADA";

        decimal total = 0;
        foreach (var item in guia.Itens)
        {
            var itemStock = await _context.ItensStock.FindAsync(item.ItemStockId);
            if (itemStock != null)
            {
                var subtotal = item.Quantidade * item.PrecoCusto;
                item.Subtotal = subtotal;
                total += subtotal;

                // ALIMENTA AUTOMATICAMENTE O STOCK PELA GUIA DE ENTRADA
                itemStock.QuantidadeAtual += item.Quantidade;
                itemStock.CustoUnitario = item.PrecoCusto;
                itemStock.UpdatedAt = DateTime.UtcNow;
            }
        }

        guia.ValorTotal = total;
        _context.GuiasEntrada.Add(guia);
        await _context.SaveChangesAsync();

        return Ok(guia);
    }
}

[ApiController]
[Route("api/[controller]")]
public class PdvController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public PdvController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    // Abertura / Fecho de Caixa e Gaveta
    [HttpPost("caixa/abrir")]
    public async Task<IActionResult> AbrirCaixa([FromBody] CaixaSessao sessao)
    {
        if (!_tenantService.EstabelecimentoId.HasValue) return BadRequest("Tenant não definido.");
        
        sessao.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        sessao.Estado = "ABERTO";
        sessao.AbertoEm = DateTime.UtcNow;

        _context.CaixasSessoes.Add(sessao);
        await _context.SaveChangesAsync();
        return Ok(sessao);
    }

    [HttpPost("caixa/fechar/{id}")]
    public async Task<IActionResult> FecharCaixa(Guid id, [FromQuery] decimal saldoFecho)
    {
        var sessao = await _context.CaixasSessoes.FindAsync(id);
        if (sessao == null) return NotFound();

        sessao.SaldoFecho = saldoFecho;
        sessao.Estado = "FECHADO";
        sessao.FechadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(sessao);
    }

    // Registo de Venda PDV ou Pedido Online do Portal do Cliente
    [HttpPost("venda")]
    public async Task<IActionResult> RegistarVenda([FromBody] PedidoVenda pedido)
    {
        if (!_tenantService.EstabelecimentoId.HasValue) return BadRequest("Tenant não definido.");

        pedido.EstabelecimentoId = _tenantService.EstabelecimentoId.Value;
        pedido.CreatedAt = DateTime.UtcNow;
        if (string.IsNullOrEmpty(pedido.NumeroPedido))
        {
            pedido.NumeroPedido = $"PED-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        decimal total = 0;
        foreach (var item in pedido.Itens)
        {
            var subtotal = item.Quantidade * item.PrecoUnitario;
            item.Subtotal = subtotal;
            total += subtotal;

            // Incrementa contador de comprados do produto
            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
            if (produto != null)
            {
                produto.TotalComprados += (int)item.Quantidade;

                // Baixa de stock associado
                if (produto.ControlaStock && produto.ItemStockId.HasValue)
                {
                    var itemStock = await _context.ItensStock.FindAsync(produto.ItemStockId.Value);
                    if (itemStock != null)
                    {
                        itemStock.QuantidadeAtual -= item.Quantidade;
                        itemStock.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }

        pedido.Subtotal = total;
        pedido.ValorTotal = total + pedido.TaxaEntrega - pedido.Desconto;

        _context.PedidosVendas.Add(pedido);
        await _context.SaveChangesAsync();

        return Ok(pedido);
    }

    [HttpGet("pedidos")]
    public async Task<IActionResult> ListarPedidos()
    {
        var pedidos = await _context.PedidosVendas
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(pedidos);
    }
}
