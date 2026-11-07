using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;
using System.Text.Json;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrudController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<CrudController> _logger;

    public CrudController(AppDbContext context, ITenantService tenantService, ILogger<CrudController> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }

    private Guid GetTenantId() => _tenantService.EstabelecimentoId ?? SyncStateController.ToGuid("1", "est");

    // =========================================================================
    // GET /api/crud/{tabela} -> Lista todos os itens da tabela (com filtro tenant)
    // =========================================================================
    [HttpGet("{tabela}")]
    public async Task<IActionResult> Listar(string tabela)
    {
        var tab = tabela.ToLower().Trim();
        try
        {
            switch (tab)
            {
                case "estabelecimentos":
                    return Ok(await _context.Estabelecimentos.AsNoTracking().ToListAsync());

                case "utilizadores":
                    var users = await _context.Utilizadores.AsNoTracking().ToListAsync();
                    return Ok(users.Select(u => new
                    {
                        u.Id,
                        Codigo = "USR-" + u.Id.ToString()[..4],
                        u.Nome,
                        Username = u.Email.Split('@')[0],
                        u.Email,
                        u.Perfil,
                        EstabelecimentoId = u.EstabelecimentoId.ToString(),
                        u.Ativo,
                        u.CreatedAt
                    }));

                case "produtos":
                    return Ok(await _context.Produtos.AsNoTracking().ToListAsync());

                case "categorias":
                    return Ok(await _context.Categorias.AsNoTracking().OrderBy(c => c.Ordem).ToListAsync());

                case "clientes":
                    return Ok(await _context.Clientes.AsNoTracking().ToListAsync());

                case "fornecedores":
                    return Ok(await _context.Fornecedores.AsNoTracking().ToListAsync());

                case "salas":
                    return Ok(await _context.Salas.AsNoTracking().ToListAsync());

                case "mesas":
                    return Ok(await _context.Mesas.AsNoTracking().ToListAsync());

                case "stock":
                case "itens_stock":
                    return Ok(await _context.ItensStock.AsNoTracking().ToListAsync());

                case "acompanhamentos":
                    return Ok(await _context.Acompanhamentos.AsNoTracking().ToListAsync());

                case "postos_preparacao":
                    return Ok(await _context.PostosPreparacao.AsNoTracking().ToListAsync());

                case "tipos_pedidos":
                    return Ok(await _context.TiposPedidos.AsNoTracking().ToListAsync());

                case "metodos_pagamento":
                    return Ok(await _context.MetodosPagamento.AsNoTracking().OrderBy(m => m.Ordem).ToListAsync());

                case "taxas_iva":
                    return Ok(await _context.TaxasIva.AsNoTracking().ToListAsync());

                case "caixas_sessoes":
                    return Ok(await _context.CaixasSessoes.AsNoTracking().ToListAsync());

                case "pedidos_vendas":
                    return Ok(await _context.PedidosVendas.AsNoTracking().OrderByDescending(p => p.CreatedAt).Take(100).ToListAsync());

                case "pedidos_online":
                    return Ok(await _context.PedidosOnline.AsNoTracking().OrderByDescending(p => p.DataPedido).Take(100).ToListAsync());

                case "unidades_medida":
                    return Ok(await _context.UnidadesMedida.AsNoTracking().ToListAsync());

                case "armazens":
                    return Ok(await _context.Armazens.AsNoTracking().ToListAsync());

                case "locais_armazenamento":
                    return Ok(await _context.LocaisArmazenamento.AsNoTracking().ToListAsync());

                case "categorias_stock":
                    return Ok(await _context.CategoriasStock.AsNoTracking().ToListAsync());

                case "compras":
                    return Ok(await _context.Compras.AsNoTracking().OrderByDescending(c => c.DataCompra).Take(100).ToListAsync());

                case "guias_entrada":
                    return Ok(await _context.GuiasEntrada.AsNoTracking().OrderByDescending(g => g.DataEmissao).Take(100).ToListAsync());

                default:
                    return NotFound(new { erro = $"Tabela '{tabela}' não encontrada." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tabela {Tabela}", tabela);
            return StatusCode(500, new { erro = ex.Message });
        }
    }

    // =========================================================================
    // GET /api/crud/{tabela}/{id} -> Obtém um registo pelo ID
    // =========================================================================
    [HttpGet("{tabela}/{id}")]
    public async Task<IActionResult> ObterPorId(string tabela, string id)
    {
        var tab = tabela.ToLower().Trim();
        var guid = SyncStateController.ToGuid(id, tab);
        try
        {
            switch (tab)
            {
                case "estabelecimentos":
                    var est = await _context.Estabelecimentos.FindAsync(guid);
                    return est != null ? Ok(est) : NotFound();

                case "utilizadores":
                    var u = await _context.Utilizadores.FindAsync(guid);
                    return u != null ? Ok(u) : NotFound();

                case "produtos":
                    var p = await _context.Produtos.FindAsync(guid);
                    return p != null ? Ok(p) : NotFound();

                case "categorias":
                    var c = await _context.Categorias.FindAsync(guid);
                    return c != null ? Ok(c) : NotFound();

                case "clientes":
                    var cli = await _context.Clientes.FindAsync(guid);
                    return cli != null ? Ok(cli) : NotFound();

                case "fornecedores":
                    var f = await _context.Fornecedores.FindAsync(guid);
                    return f != null ? Ok(f) : NotFound();

                case "salas":
                    var s = await _context.Salas.FindAsync(guid);
                    return s != null ? Ok(s) : NotFound();

                case "mesas":
                    var m = await _context.Mesas.FindAsync(guid);
                    return m != null ? Ok(m) : NotFound();

                case "stock":
                case "itens_stock":
                    var stk = await _context.ItensStock.FindAsync(guid);
                    return stk != null ? Ok(stk) : NotFound();

                case "acompanhamentos":
                    var a = await _context.Acompanhamentos.FindAsync(guid);
                    return a != null ? Ok(a) : NotFound();

                case "postos_preparacao":
                    var post = await _context.PostosPreparacao.FindAsync(guid);
                    return post != null ? Ok(post) : NotFound();

                case "tipos_pedidos":
                    var tp = await _context.TiposPedidos.FindAsync(guid);
                    return tp != null ? Ok(tp) : NotFound();

                case "metodos_pagamento":
                    var met = await _context.MetodosPagamento.FindAsync(guid);
                    return met != null ? Ok(met) : NotFound();

                case "taxas_iva":
                    var tx = await _context.TaxasIva.FindAsync(guid);
                    return tx != null ? Ok(tx) : NotFound();

                default:
                    return NotFound(new { erro = $"Tabela '{tabela}' não encontrada." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter registo {Id} da tabela {Tabela}", id, tabela);
            return StatusCode(500, new { erro = ex.Message });
        }
    }

    // =========================================================================
    // POST /api/crud/{tabela} -> Cria um novo registo
    // =========================================================================
    [HttpPost("{tabela}")]
    public async Task<IActionResult> Criar(string tabela, [FromBody] JsonElement payload)
    {
        var tab = tabela.ToLower().Trim();
        var estId = GetTenantId();
        try
        {
            var idStr = payload.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            var guid = SyncStateController.ToGuid(idStr, tab);

            switch (tab)
            {
                case "produtos":
                    var prod = new Produto
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        CodigoProduto = payload.TryGetProperty("codigo_produto", out var cp) ? cp.GetString() ?? "PRD-001" : "PRD-001",
                        Nome = payload.TryGetProperty("nome", out var np) ? np.GetString() ?? "" : "",
                        Preco = payload.TryGetProperty("preco", out var pr) ? (decimal)pr.GetDouble() : 0m,
                        ImagemUrl = payload.TryGetProperty("imagem_url", out var img) ? img.GetString() : null,
                        Estado = payload.TryGetProperty("estado", out var st) ? st.GetString() ?? "disponivel" : "disponivel",
                        EmDestaque = payload.TryGetProperty("em_destaque", out var d) && d.GetBoolean(),
                        TemPromocao = payload.TryGetProperty("tem_promocao", out var tp) && tp.GetBoolean(),
                        PrecoPromocional = payload.TryGetProperty("preco_promocional", out var pp) && pp.ValueKind == JsonValueKind.Number ? (decimal)pp.GetDouble() : null,
                        CreatedAt = DateTime.UtcNow
                    };
                    if (payload.TryGetProperty("categoria_id", out var cid) && Guid.TryParse(cid.GetString(), out var catG))
                        prod.CategoriaId = catG;
                    _context.Produtos.Add(prod);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/produtos/{prod.Id}", prod);

                case "categorias":
                    var cat = new Categoria
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var cc) ? cc.GetString() ?? "CAT-01" : "CAT-01",
                        Nome = payload.TryGetProperty("nome", out var nc) ? nc.GetString() ?? "" : "",
                        Emoji = payload.TryGetProperty("emoji", out var em) ? em.GetString() ?? "🍽️" : "🍽️",
                        Ordem = payload.TryGetProperty("ordem", out var ord) ? ord.GetInt32() : 0,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Categorias.Add(cat);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/categorias/{cat.Id}", cat);

                case "clientes":
                    var cli = new Cliente
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var ncli) ? ncli.GetString() ?? "" : "",
                        Telefone = payload.TryGetProperty("telefone", out var tcli) ? tcli.GetString() ?? "" : "",
                        Email = payload.TryGetProperty("email", out var ecli) ? ecli.GetString() : null,
                        Nif = payload.TryGetProperty("nuit", out var n1) ? n1.GetString() : (payload.TryGetProperty("nif", out var n2) ? n2.GetString() : null),
                        Morada = payload.TryGetProperty("morada", out var m1) ? m1.GetString() : (payload.TryGetProperty("endereco", out var m2) ? m2.GetString() : null),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Clientes.Add(cli);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/clientes/{cli.Id}", cli);

                case "fornecedores":
                    var forn = new Fornecedor
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var nforn) ? nforn.GetString() ?? "" : "",
                        Telefone = payload.TryGetProperty("telefone", out var tforn) ? tforn.GetString() : null,
                        Email = payload.TryGetProperty("email", out var eforn) ? eforn.GetString() : null,
                        Nif = payload.TryGetProperty("nuit", out var fn1) ? fn1.GetString() : (payload.TryGetProperty("nif", out var fn2) ? fn2.GetString() : null),
                        ContactoResponsavel = payload.TryGetProperty("contacto", out var cr) ? cr.GetString() : null,
                        Morada = payload.TryGetProperty("morada", out var mf) ? mf.GetString() : null,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Fornecedores.Add(forn);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/fornecedores/{forn.Id}", forn);

                case "utilizadores":
                    var user = new Utilizador
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var nu) ? nu.GetString() ?? "" : "",
                        Email = payload.TryGetProperty("email", out var eu) ? eu.GetString() ?? "" : "",
                        Perfil = payload.TryGetProperty("perfil", out var pu) ? pu.GetString() ?? "OPERADOR" : "OPERADOR",
                        Ativo = !payload.TryGetProperty("ativo", out var au) || au.GetBoolean(),
                        SenhaHash = "AQAAAAEAACcQAAAAEJ...",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Utilizadores.Add(user);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/utilizadores/{user.Id}", user);

                case "salas":
                    var sala = new Sala
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var ns) ? ns.GetString() ?? "" : "",
                        Descricao = payload.TryGetProperty("descricao", out var ds) ? ds.GetString() : null,
                        CapacidadeMesas = payload.TryGetProperty("capacidade_mesas", out var cms) ? cms.GetInt32() : 0
                    };
                    _context.Salas.Add(sala);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/salas/{sala.Id}", sala);

                case "mesas":
                    var mesa = new Mesa
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        NumeroMesa = payload.TryGetProperty("numero_mesa", out var num) ? num.GetString() ?? "" : "",
                        CapacidadeLugares = payload.TryGetProperty("capacidade_lugares", out var cap) ? cap.GetInt32() : 4,
                        Estado = payload.TryGetProperty("estado", out var stm) ? stm.GetString() ?? "LIVRE" : "LIVRE",
                        QrCodeToken = payload.TryGetProperty("qrcode_token", out var qr) ? qr.GetString() : null
                    };
                    if (payload.TryGetProperty("sala_id", out var sid) && Guid.TryParse(sid.GetString(), out var salaG))
                        mesa.SalaId = salaG;
                    _context.Mesas.Add(mesa);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/mesas/{mesa.Id}", mesa);

                case "stock":
                case "itens_stock":
                    var itemStock = new ItemStock
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var nstk) ? nstk.GetString() ?? "" : "",
                        UnidadeMedida = payload.TryGetProperty("unidade", out var un) ? un.GetString() ?? "UN" : "UN",
                        QuantidadeAtual = payload.TryGetProperty("qtd", out var q) ? (decimal)q.GetDouble() : 0m,
                        QuantidadeMinima = payload.TryGetProperty("minimo", out var qm) ? (decimal)qm.GetDouble() : 0m,
                        CustoUnitario = payload.TryGetProperty("custo", out var cu) ? (decimal)cu.GetDouble() : 0m,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ItensStock.Add(itemStock);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/stock/{itemStock.Id}", itemStock);

                case "acompanhamentos":
                    var acomp = new Acompanhamento
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var na) ? na.GetString() ?? "" : "",
                        TipoSelecao = payload.TryGetProperty("tipo", out var ta) ? ta.GetString() ?? "opcional_pago" : "opcional_pago",
                        PrecoAdicional = payload.TryGetProperty("preco_adicional", out var pa) ? (decimal)pa.GetDouble() : 0m,
                        Disponivel = !payload.TryGetProperty("disponivel", out var da) || da.GetBoolean()
                    };
                    _context.Acompanhamentos.Add(acomp);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/acompanhamentos/{acomp.Id}", acomp);

                case "taxas_iva":
                    var taxa = new TaxaIva
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var civa) ? civa.GetString() ?? "IVA-16" : "IVA-16",
                        Iva = payload.TryGetProperty("iva", out var v) ? (decimal)v.GetDouble() : 16.00m,
                        Moeda = payload.TryGetProperty("moeda", out var mo) ? mo.GetString() ?? "MZN" : "MZN",
                        Descricao = payload.TryGetProperty("descricao", out var dt) ? dt.GetString() : null,
                        Ativo = !payload.TryGetProperty("ativo", out var atx) || atx.GetBoolean(),
                        Padrao = payload.TryGetProperty("padrao", out var pd) && pd.GetBoolean(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.TaxasIva.Add(taxa);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/taxas_iva/{taxa.Id}", taxa);

                default:
                    return NotFound(new { erro = $"Criação para tabela '{tabela}' não implementada." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar registo na tabela {Tabela}", tabela);
            return StatusCode(500, new { erro = ex.Message });
        }
    }

    // =========================================================================
    // PUT /api/crud/{tabela}/{id} -> Atualiza um registo existente
    // =========================================================================
    [HttpPut("{tabela}/{id}")]
    public async Task<IActionResult> Atualizar(string tabela, string id, [FromBody] JsonElement payload)
    {
        var tab = tabela.ToLower().Trim();
        var guid = SyncStateController.ToGuid(id, tab);
        try
        {
            switch (tab)
            {
                case "produtos":
                    var prod = await _context.Produtos.FindAsync(guid);
                    if (prod == null) return NotFound(new { erro = "Produto não encontrado." });
                    if (payload.TryGetProperty("codigo_produto", out var cp)) prod.CodigoProduto = cp.GetString() ?? prod.CodigoProduto;
                    if (payload.TryGetProperty("nome", out var np)) prod.Nome = np.GetString() ?? prod.Nome;
                    if (payload.TryGetProperty("preco", out var pr)) prod.Preco = (decimal)pr.GetDouble();
                    if (payload.TryGetProperty("preco_promocional", out var pp)) prod.PrecoPromocional = pp.ValueKind == JsonValueKind.Number ? (decimal)pp.GetDouble() : null;
                    if (payload.TryGetProperty("tem_promocao", out var tp)) prod.TemPromocao = tp.GetBoolean();
                    if (payload.TryGetProperty("imagem_url", out var img)) prod.ImagemUrl = img.GetString();
                    if (payload.TryGetProperty("estado", out var st)) prod.Estado = st.GetString() ?? prod.Estado;
                    if (payload.TryGetProperty("em_destaque", out var d)) prod.EmDestaque = d.GetBoolean();
                    if (payload.TryGetProperty("categoria_id", out var cid) && Guid.TryParse(cid.GetString(), out var catG))
                        prod.CategoriaId = catG;
                    await _context.SaveChangesAsync();
                    return Ok(prod);

                case "categorias":
                    var cat = await _context.Categorias.FindAsync(guid);
                    if (cat == null) return NotFound(new { erro = "Categoria não encontrada." });
                    if (payload.TryGetProperty("codigo", out var cc)) cat.Codigo = cc.GetString() ?? cat.Codigo;
                    if (payload.TryGetProperty("nome", out var nc)) cat.Nome = nc.GetString() ?? cat.Nome;
                    if (payload.TryGetProperty("emoji", out var em)) cat.Emoji = em.GetString() ?? cat.Emoji;
                    if (payload.TryGetProperty("ordem", out var ord)) cat.Ordem = ord.GetInt32();
                    await _context.SaveChangesAsync();
                    return Ok(cat);

                case "clientes":
                    var cli = await _context.Clientes.FindAsync(guid);
                    if (cli == null) return NotFound(new { erro = "Cliente não encontrado." });
                    if (payload.TryGetProperty("nome", out var ncli)) cli.Nome = ncli.GetString() ?? cli.Nome;
                    if (payload.TryGetProperty("telefone", out var tcli)) cli.Telefone = tcli.GetString() ?? cli.Telefone;
                    if (payload.TryGetProperty("email", out var ecli)) cli.Email = ecli.GetString();
                    if (payload.TryGetProperty("nuit", out var n1)) cli.Nif = n1.GetString();
                    else if (payload.TryGetProperty("nif", out var n2)) cli.Nif = n2.GetString();
                    if (payload.TryGetProperty("morada", out var m1)) cli.Morada = m1.GetString();
                    else if (payload.TryGetProperty("endereco", out var m2)) cli.Morada = m2.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(cli);

                case "fornecedores":
                    var forn = await _context.Fornecedores.FindAsync(guid);
                    if (forn == null) return NotFound(new { erro = "Fornecedor não encontrado." });
                    if (payload.TryGetProperty("nome", out var nforn)) forn.Nome = nforn.GetString() ?? forn.Nome;
                    if (payload.TryGetProperty("telefone", out var tforn)) forn.Telefone = tforn.GetString();
                    if (payload.TryGetProperty("email", out var eforn)) forn.Email = eforn.GetString();
                    if (payload.TryGetProperty("nuit", out var fn1)) forn.Nif = fn1.GetString();
                    else if (payload.TryGetProperty("nif", out var fn2)) forn.Nif = fn2.GetString();
                    if (payload.TryGetProperty("contacto", out var cr)) forn.ContactoResponsavel = cr.GetString();
                    if (payload.TryGetProperty("morada", out var mf)) forn.Morada = mf.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(forn);

                case "utilizadores":
                    var user = await _context.Utilizadores.FindAsync(guid);
                    if (user == null) return NotFound(new { erro = "Utilizador não encontrado." });
                    if (payload.TryGetProperty("nome", out var nu)) user.Nome = nu.GetString() ?? user.Nome;
                    if (payload.TryGetProperty("email", out var eu)) user.Email = eu.GetString() ?? user.Email;
                    if (payload.TryGetProperty("perfil", out var pu)) user.Perfil = pu.GetString() ?? user.Perfil;
                    if (payload.TryGetProperty("ativo", out var au)) user.Ativo = au.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(user);

                case "stock":
                case "itens_stock":
                    var stk = await _context.ItensStock.FindAsync(guid);
                    if (stk == null) return NotFound(new { erro = "Item de stock não encontrado." });
                    if (payload.TryGetProperty("nome", out var nstk)) stk.Nome = nstk.GetString() ?? stk.Nome;
                    if (payload.TryGetProperty("unidade", out var un)) stk.UnidadeMedida = un.GetString() ?? stk.UnidadeMedida;
                    if (payload.TryGetProperty("qtd", out var q)) stk.QuantidadeAtual = (decimal)q.GetDouble();
                    if (payload.TryGetProperty("minimo", out var qm)) stk.QuantidadeMinima = (decimal)qm.GetDouble();
                    if (payload.TryGetProperty("custo", out var cu)) stk.CustoUnitario = (decimal)cu.GetDouble();
                    await _context.SaveChangesAsync();
                    return Ok(stk);

                case "mesas":
                    var mesa = await _context.Mesas.FindAsync(guid);
                    if (mesa == null) return NotFound(new { erro = "Mesa não encontrada." });
                    if (payload.TryGetProperty("numero_mesa", out var num)) mesa.NumeroMesa = num.GetString() ?? mesa.NumeroMesa;
                    if (payload.TryGetProperty("capacidade_lugares", out var cap)) mesa.CapacidadeLugares = cap.GetInt32();
                    if (payload.TryGetProperty("estado", out var stm)) mesa.Estado = stm.GetString() ?? mesa.Estado;
                    if (payload.TryGetProperty("qrcode_token", out var qr)) mesa.QrCodeToken = qr.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(mesa);

                case "salas":
                    var sala = await _context.Salas.FindAsync(guid);
                    if (sala == null) return NotFound(new { erro = "Sala não encontrada." });
                    if (payload.TryGetProperty("nome", out var ns)) sala.Nome = ns.GetString() ?? sala.Nome;
                    if (payload.TryGetProperty("descricao", out var ds)) sala.Descricao = ds.GetString();
                    if (payload.TryGetProperty("capacidade_mesas", out var cms)) sala.CapacidadeMesas = cms.GetInt32();
                    await _context.SaveChangesAsync();
                    return Ok(sala);

                case "taxas_iva":
                    var taxa = await _context.TaxasIva.FindAsync(guid);
                    if (taxa == null) return NotFound(new { erro = "Taxa de IVA não encontrada." });
                    if (payload.TryGetProperty("codigo", out var civa)) taxa.Codigo = civa.GetString() ?? taxa.Codigo;
                    if (payload.TryGetProperty("iva", out var v)) taxa.Iva = (decimal)v.GetDouble();
                    if (payload.TryGetProperty("moeda", out var mo)) taxa.Moeda = mo.GetString() ?? taxa.Moeda;
                    if (payload.TryGetProperty("descricao", out var dt)) taxa.Descricao = dt.GetString();
                    if (payload.TryGetProperty("ativo", out var atx)) taxa.Ativo = atx.GetBoolean();
                    if (payload.TryGetProperty("padrao", out var pd)) taxa.Padrao = pd.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(taxa);

                default:
                    return NotFound(new { erro = $"Atualização para tabela '{tabela}' não implementada." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar registo {Id} da tabela {Tabela}", id, tabela);
            return StatusCode(500, new { erro = ex.Message });
        }
    }

    // =========================================================================
    // DELETE /api/crud/{tabela}/{id} -> Elimina um registo diretamente da BD
    // =========================================================================
    [HttpDelete("{tabela}/{id}")]
    public async Task<IActionResult> Eliminar(string tabela, string id)
    {
        var tab = tabela.ToLower().Trim();
        var guid = SyncStateController.ToGuid(id, tab);
        try
        {
            switch (tab)
            {
                case "produtos":
                    var prod = await _context.Produtos.FindAsync(guid);
                    if (prod == null) return NotFound(new { erro = "Produto não encontrado." });
                    _context.Produtos.Remove(prod);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Produto '{prod.Nome}' eliminado com sucesso." });

                case "categorias":
                    var cat = await _context.Categorias.FindAsync(guid);
                    if (cat == null) return NotFound(new { erro = "Categoria não encontrada." });
                    _context.Categorias.Remove(cat);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Categoria '{cat.Nome}' eliminada com sucesso." });

                case "clientes":
                    var cli = await _context.Clientes.FindAsync(guid);
                    if (cli == null) return NotFound(new { erro = "Cliente não encontrado." });
                    _context.Clientes.Remove(cli);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Cliente '{cli.Nome}' eliminado com sucesso." });

                case "fornecedores":
                    var forn = await _context.Fornecedores.FindAsync(guid);
                    if (forn == null) return NotFound(new { erro = "Fornecedor não encontrado." });
                    _context.Fornecedores.Remove(forn);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Fornecedor '{forn.Nome}' eliminado com sucesso." });

                case "utilizadores":
                    var user = await _context.Utilizadores.FindAsync(guid);
                    if (user == null) return NotFound(new { erro = "Utilizador não encontrado." });
                    _context.Utilizadores.Remove(user);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Utilizador '{user.Nome}' eliminado com sucesso." });

                case "salas":
                    var sala = await _context.Salas.FindAsync(guid);
                    if (sala == null) return NotFound(new { erro = "Sala não encontrada." });
                    _context.Salas.Remove(sala);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Sala '{sala.Nome}' eliminada com sucesso." });

                case "mesas":
                    var mesa = await _context.Mesas.FindAsync(guid);
                    if (mesa == null) return NotFound(new { erro = "Mesa não encontrada." });
                    _context.Mesas.Remove(mesa);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Mesa '{mesa.NumeroMesa}' eliminada com sucesso." });

                case "stock":
                case "itens_stock":
                    var stk = await _context.ItensStock.FindAsync(guid);
                    if (stk == null) return NotFound(new { erro = "Item de stock não encontrado." });
                    _context.ItensStock.Remove(stk);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Item de stock '{stk.Nome}' eliminado com sucesso." });

                case "acompanhamentos":
                    var acomp = await _context.Acompanhamentos.FindAsync(guid);
                    if (acomp == null) return NotFound(new { erro = "Acompanhamento não encontrado." });
                    _context.Acompanhamentos.Remove(acomp);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Acompanhamento '{acomp.Nome}' eliminado com sucesso." });

                case "taxas_iva":
                    var taxa = await _context.TaxasIva.FindAsync(guid);
                    if (taxa == null) return NotFound(new { erro = "Taxa de IVA não encontrada." });
                    _context.TaxasIva.Remove(taxa);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Taxa de IVA '{taxa.Codigo}' eliminada com sucesso." });

                case "metodos_pagamento":
                    var met = await _context.MetodosPagamento.FindAsync(guid);
                    if (met == null) return NotFound(new { erro = "Método de pagamento não encontrado." });
                    _context.MetodosPagamento.Remove(met);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Método '{met.Nome}' eliminado com sucesso." });

                default:
                    return NotFound(new { erro = $"Eliminação para tabela '{tabela}' não implementada." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao eliminar registo {Id} da tabela {Tabela}", id, tabela);
            return StatusCode(500, new { erro = ex.Message });
        }
    }
}
