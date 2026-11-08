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
                        Codigo = !string.IsNullOrEmpty(u.Codigo) ? u.Codigo : ("USR-" + u.Id.ToString()[..4]),
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
                case "guias_entradas":
                    return Ok(await _context.GuiasEntrada.AsNoTracking().OrderByDescending(g => g.DataEmissao).Take(100).ToListAsync());

                case "canais_pagamento":
                    return Ok(await _context.CanaisPagamento.AsNoTracking().ToListAsync());

                case "itens_compra":
                    return Ok(await _context.ItensCompra.AsNoTracking().ToListAsync());

                case "itens_guia_entrada":
                case "itens_guias_entrada":
                    return Ok(await _context.ItensGuiaEntrada.AsNoTracking().ToListAsync());

                case "itens_pedidos_vendas":
                case "itens_pedido_venda":
                    return Ok(await _context.ItensPedidoVenda.AsNoTracking().ToListAsync());

                case "tabela_precos_fornecedores":
                case "tabela_precos":
                case "precos_fornecedores":
                    return Ok(await _context.TabelaPrecosFornecedores.AsNoTracking().OrderBy(tp => tp.ProdutoNome).ToListAsync());

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

                case "armazens":
                    var arm = await _context.Armazens.FindAsync(guid);
                    return arm != null ? Ok(arm) : NotFound();

                case "caixas_sessoes":
                case "caixa_sessoes":
                    var cs = await _context.CaixasSessoes.FindAsync(guid);
                    return cs != null ? Ok(cs) : NotFound();

                case "canais_pagamento":
                    var cpItem = await _context.CanaisPagamento.FindAsync(guid);
                    return cpItem != null ? Ok(cpItem) : NotFound();

                case "categorias_stock":
                    var csStock = await _context.CategoriasStock.FindAsync(guid);
                    return csStock != null ? Ok(csStock) : NotFound();

                case "compras":
                    var cmp = await _context.Compras.FindAsync(guid);
                    return cmp != null ? Ok(cmp) : NotFound();

                case "guias_entrada":
                case "guias_entradas":
                    var ge = await _context.GuiasEntrada.FindAsync(guid);
                    return ge != null ? Ok(ge) : NotFound();

                case "itens_compra":
                    var icmp = await _context.ItensCompra.FindAsync(guid);
                    return icmp != null ? Ok(icmp) : NotFound();

                case "itens_guia_entrada":
                case "itens_guias_entrada":
                    var ige = await _context.ItensGuiaEntrada.FindAsync(guid);
                    return ige != null ? Ok(ige) : NotFound();

                case "itens_pedidos_vendas":
                case "itens_pedido_venda":
                    var ipv = await _context.ItensPedidoVenda.FindAsync(guid);
                    return ipv != null ? Ok(ipv) : NotFound();

                case "locais_armazenamento":
                    var loc = await _context.LocaisArmazenamento.FindAsync(guid);
                    return loc != null ? Ok(loc) : NotFound();

                case "pedidos_online":
                    var po = await _context.PedidosOnline.FindAsync(guid);
                    return po != null ? Ok(po) : NotFound();

                case "pedidos_vendas":
                    var pv = await _context.PedidosVendas.FindAsync(guid);
                    return pv != null ? Ok(pv) : NotFound();

                case "unidades_medida":
                case "unidades_medidas":
                    var um = await _context.UnidadesMedida.FindAsync(guid);
                    return um != null ? Ok(um) : NotFound();

                case "tabela_precos_fornecedores":
                case "tabela_precos":
                case "precos_fornecedores":
                    var tpItem = await _context.TabelaPrecosFornecedores.FindAsync(guid);
                    return tpItem != null ? Ok(tpItem) : NotFound();

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
    // MÉTODOS AUXILIARES DE VALIDAÇÃO (Regras de Negócio Globais)
    // =========================================================================
    private static string? ValidarEmail(string? email, bool obrigatorio = false)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return obrigatorio ? "O email é obrigatório." : null;
        }
        email = email.Trim();
        if (!email.Contains('@') || email.IndexOf('@') == 0 || email.IndexOf('@') == email.Length - 1)
        {
            return "Email inválido. Deve conter o caractere '@' e um domínio válido.";
        }
        return null;
    }

    private static string? ValidarTelefone(string? telefone, bool obrigatorio = false)
    {
        if (string.IsNullOrWhiteSpace(telefone))
        {
            return obrigatorio ? "O telefone é obrigatório." : null;
        }
        var digitos = new string(telefone.Where(char.IsDigit).ToArray());
        // Suporta telefones locais de 10 dígitos (ex: 8412345678) ou formato internacional de até 14 caracteres/dígitos (ex: +258 84 000 0000)
        if (digitos.Length < 9 || digitos.Length > 14)
        {
            return $"O telefone deve conter entre 9 e 14 dígitos (encontrados {digitos.Length}). Ex: 8412345678 ou +258 84 000 0000.";
        }
        return null;
    }

    private static string? ValidarNuit(string? nuit, bool obrigatorio = false)
    {
        if (string.IsNullOrWhiteSpace(nuit))
        {
            return obrigatorio ? "O NUIT é obrigatório e deve ter exactamente 10 dígitos." : null;
        }
        var digitos = new string(nuit.Where(char.IsDigit).ToArray());
        if (digitos.Length != 10)
        {
            return $"O NUIT deve conter exactamente 10 dígitos (encontrados {digitos.Length}). Ex: 4001234567.";
        }
        return null;
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
                    var catNome = payload.TryGetProperty("nome", out var nc) ? nc.GetString()?.Trim() ?? "" : "";
                    if (string.IsNullOrWhiteSpace(catNome))
                    {
                        return BadRequest(new { erro = "O nome da categoria é obrigatório." });
                    }
                    // Validação de unicidade do nome da categoria
                    var existeCat = await _context.Categorias.AsNoTracking().AnyAsync(c => c.Nome.ToLower() == catNome.ToLower());
                    if (existeCat)
                    {
                        return BadRequest(new { erro = $"Já existe uma categoria registada com o nome '{catNome}'." });
                    }
                    var cat = new Categoria
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var cc) ? cc.GetString() ?? "CAT-01" : "CAT-01",
                        Nome = catNome,
                        Emoji = payload.TryGetProperty("emoji", out var em) ? em.GetString() ?? "🍽️" : "🍽️",
                        Ordem = payload.TryGetProperty("ordem", out var ord) ? ord.GetInt32() : 0,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Categorias.Add(cat);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/categorias/{cat.Id}", cat);

                case "clientes":
                    var cliEmail = payload.TryGetProperty("email", out var ecli) ? ecli.GetString()?.Trim() : null;
                    var errEmailCli = ValidarEmail(cliEmail, obrigatorio: false);
                    if (errEmailCli != null) return BadRequest(new { erro = errEmailCli });

                    var cliTel = payload.TryGetProperty("telefone", out var tcli) ? tcli.GetString()?.Trim() : null;
                    var errTelCli = ValidarTelefone(cliTel, obrigatorio: false);
                    if (errTelCli != null) return BadRequest(new { erro = errTelCli });

                    var cliNuit = payload.TryGetProperty("nuit", out var n1) ? n1.GetString()?.Trim() : (payload.TryGetProperty("nif", out var n2) ? n2.GetString()?.Trim() : null);
                    var errNuitCli = ValidarNuit(cliNuit, obrigatorio: false);
                    if (errNuitCli != null) return BadRequest(new { erro = errNuitCli });

                    var cli = new Cliente
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var codCli) ? codCli.GetString() ?? ("CLI-" + guid.ToString()[..4]) : ("CLI-" + guid.ToString()[..4]),
                        Nome = payload.TryGetProperty("nome", out var ncli) ? ncli.GetString() ?? "" : "",
                        Telefone = cliTel ?? "",
                        Email = cliEmail,
                        Nif = cliNuit,
                        Morada = payload.TryGetProperty("morada", out var m1) ? m1.GetString() : (payload.TryGetProperty("endereco", out var m2) ? m2.GetString() : null),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Clientes.Add(cli);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/clientes/{cli.Id}", cli);

                case "fornecedores":
                    var fornEmail = payload.TryGetProperty("email", out var eforn) ? eforn.GetString()?.Trim() : null;
                    var errEmailForn = ValidarEmail(fornEmail, obrigatorio: false);
                    if (errEmailForn != null) return BadRequest(new { erro = errEmailForn });

                    var fornTel = payload.TryGetProperty("telefone", out var tforn) ? tforn.GetString()?.Trim() : null;
                    var errTelForn = ValidarTelefone(fornTel, obrigatorio: false);
                    if (errTelForn != null) return BadRequest(new { erro = errTelForn });

                    var fornNuit = payload.TryGetProperty("nuit", out var fn1) ? fn1.GetString()?.Trim() : (payload.TryGetProperty("nif", out var fn2) ? fn2.GetString()?.Trim() : null);
                    var errNuitForn = ValidarNuit(fornNuit, obrigatorio: false);
                    if (errNuitForn != null) return BadRequest(new { erro = errNuitForn });

                    var forn = new Fornecedor
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var codForn) ? codForn.GetString() ?? ("FORN-" + guid.ToString()[..4]) : ("FORN-" + guid.ToString()[..4]),
                        Nome = payload.TryGetProperty("nome", out var nforn) ? nforn.GetString() ?? "" : "",
                        Telefone = fornTel,
                        Email = fornEmail,
                        Nif = fornNuit,
                        ContactoResponsavel = payload.TryGetProperty("contacto", out var cr) ? cr.GetString() : null,
                        Morada = payload.TryGetProperty("morada", out var mf) ? mf.GetString() : null,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Fornecedores.Add(forn);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/fornecedores/{forn.Id}", forn);

                case "utilizadores":
                    var userEmail = payload.TryGetProperty("email", out var eu) ? eu.GetString()?.Trim() ?? "" : "";
                    var errEmailUser = ValidarEmail(userEmail, obrigatorio: true);
                    if (errEmailUser != null) return BadRequest(new { erro = errEmailUser });

                    var user = new Utilizador
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var codUser) ? codUser.GetString() ?? ("USR-" + guid.ToString()[..4]) : ("USR-" + guid.ToString()[..4]),
                        Nome = payload.TryGetProperty("nome", out var nu) ? nu.GetString() ?? "" : "",
                        Email = userEmail,
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

                case "postos_preparacao":
                    var posto = new PostoPreparacao
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var _c_ppc) ? _c_ppc.GetString() ?? ("PST-" + guid.ToString()[..4].ToUpper()) : ("PST-" + guid.ToString()[..4].ToUpper()),
                        Descricao = payload.TryGetProperty("descricao", out var _c_ppd) ? _c_ppd.GetString() ?? (payload.TryGetProperty("nome", out var _c_ppn) ? _c_ppn.GetString() ?? "Posto" : "Posto") : (payload.TryGetProperty("nome", out var _c_ppn2) ? _c_ppn2.GetString() ?? "Posto" : "Posto"),
                        Ativo = !payload.TryGetProperty("ativo", out var _c_ppa) || _c_ppa.GetBoolean()
                    };
                    _context.PostosPreparacao.Add(posto);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/postos_preparacao/{posto.Id}", posto);

                case "tipos_pedidos":
                    var tipoPed = new TipoPedido
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var _c_tpc) ? _c_tpc.GetString() ?? ("TP-" + guid.ToString()[..4].ToUpper()) : ("TP-" + guid.ToString()[..4].ToUpper()),
                        Descricao = payload.TryGetProperty("descricao", out var _c_tpd) ? _c_tpd.GetString() ?? (payload.TryGetProperty("nome", out var _c_tpn) ? _c_tpn.GetString() ?? "Tipo Pedido" : "Tipo Pedido") : (payload.TryGetProperty("nome", out var _c_tpn2) ? _c_tpn2.GetString() ?? "Tipo Pedido" : "Tipo Pedido"),
                        Ativo = !payload.TryGetProperty("ativo", out var _c_tpa) || _c_tpa.GetBoolean()
                    };
                    _context.TiposPedidos.Add(tipoPed);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/tipos_pedidos/{tipoPed.Id}", tipoPed);

                case "metodos_pagamento":
                    var metPag = new MetodoPagamento
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var _c_mpc) ? _c_mpc.GetString() ?? ("MET-" + guid.ToString()[..4].ToUpper()) : ("MET-" + guid.ToString()[..4].ToUpper()),
                        Nome = payload.TryGetProperty("nome", out var _c_mpn) ? _c_mpn.GetString() ?? "Pagamento" : "Pagamento",
                        Icone = payload.TryGetProperty("icone", out var _c_mpi) ? _c_mpi.GetString() ?? "fa-solid fa-money-bill" : "fa-solid fa-money-bill",
                        TaxaPercentual = payload.TryGetProperty("taxa_percentual", out var _c_mptx) ? (decimal)_c_mptx.GetDouble() : 0,
                        PermiteTroco = !payload.TryGetProperty("permite_troco", out var _c_mppt) || _c_mppt.GetBoolean(),
                        Ordem = payload.TryGetProperty("ordem", out var _c_mpo) ? _c_mpo.GetInt32() : 1,
                        Ativo = !payload.TryGetProperty("ativo", out var _c_mpa) || _c_mpa.GetBoolean()
                    };
                    _context.MetodosPagamento.Add(metPag);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/metodos_pagamento/{metPag.Id}", metPag);

                case "armazens":
                    var armazem = new Armazem
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var _c_armCod) ? _c_armCod.GetString() ?? ("ARM-" + guid.ToString()[..4].ToUpper()) : ("ARM-" + guid.ToString()[..4].ToUpper()),
                        Nome = payload.TryGetProperty("nome", out var _c_armNome) ? _c_armNome.GetString() ?? "Armazém Principal" : "Armazém Principal",
                        Tipo = payload.TryGetProperty("tipo", out var _c_armTipo) ? _c_armTipo.GetString() ?? "GERAL" : "GERAL",
                        Ativo = !payload.TryGetProperty("ativo", out var _c_armAt) || _c_armAt.GetBoolean()
                    };
                    _context.Armazens.Add(armazem);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/armazens/{armazem.Id}", armazem);

                case "caixas_sessoes":
                case "caixa_sessoes":
                    var caixaSessao = new CaixaSessao
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_csCod) ? _c_csCod.GetString() : ("CXS-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        UtilizadorId = payload.TryGetProperty("utilizador_id", out var _c_uid) && Guid.TryParse(_c_uid.GetString(), out var _c_ug) ? _c_ug : estId,
                        SaldoAbertura = payload.TryGetProperty("saldo_abertura", out var _c_sa) ? (decimal)_c_sa.GetDouble() : 1500.00m,
                        SaldoFecho = payload.TryGetProperty("saldo_fecho", out var _c_sf) ? (decimal?)_c_sf.GetDouble() : null,
                        Estado = payload.TryGetProperty("estado", out var _c_stCs) ? _c_stCs.GetString() ?? "ABERTO" : "ABERTO",
                        AbertoEm = DateTime.UtcNow,
                        Observacoes = payload.TryGetProperty("observacoes", out var _c_obsCs) ? _c_obsCs.GetString() : null
                    };
                    _context.CaixasSessoes.Add(caixaSessao);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/caixas_sessoes/{caixaSessao.Id}", caixaSessao);

                case "canais_pagamento":
                    var canal = new CanalPagamento
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_cpCod) ? _c_cpCod.GetString() : ("CP-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        CodigoOperadora = payload.TryGetProperty("codigo_operadora", out var _c_co) ? _c_co.GetString() ?? "MPESA" : "MPESA",
                        Descricao = payload.TryGetProperty("descricao", out var _c_cd) ? _c_cd.GetString() ?? "Conta Móvel" : "Conta Móvel",
                        NumeroAssociado = payload.TryGetProperty("numero_associado", out var _c_numAssoc) ? _c_numAssoc.GetString() ?? "" : "",
                        TitularConta = payload.TryGetProperty("titular_conta", out var _c_tc) ? _c_tc.GetString() : null,
                        InstrucoesPagamento = payload.TryGetProperty("instrucoes_pagamento", out var _c_ip) ? _c_ip.GetString() : null,
                        Ativo = !payload.TryGetProperty("ativo", out var _c_atCp) || _c_atCp.GetBoolean(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.CanaisPagamento.Add(canal);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/canais_pagamento/{canal.Id}", canal);

                case "categorias_stock":
                    var catStock = new CategoriaStock
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_cskCod) ? _c_cskCod.GetString() : ("CAT-STK-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        Nome = payload.TryGetProperty("nome", out var _c_cskNome) ? _c_cskNome.GetString() ?? "Geral" : "Geral",
                        Descricao = payload.TryGetProperty("descricao", out var _c_cskDesc) ? _c_cskDesc.GetString() : null
                    };
                    _context.CategoriasStock.Add(catStock);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/categorias_stock/{catStock.Id}", catStock);

                case "compras":
                    var compra = new Compra
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_cmpCod) ? _c_cmpCod.GetString() : ("CMP-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        FornecedorId = payload.TryGetProperty("fornecedor_id", out var _c_fid) && Guid.TryParse(_c_fid.GetString(), out var _c_fg) ? _c_fg : null,
                        NumeroFatura = payload.TryGetProperty("numero_fatura", out var _c_nf) ? _c_nf.GetString() : null,
                        DataCompra = DateTime.UtcNow,
                        ValorTotal = payload.TryGetProperty("valor_total", out var _c_vt) ? (decimal)_c_vt.GetDouble() : 0,
                        Status = payload.TryGetProperty("status", out var _c_stCmp) ? _c_stCmp.GetString() ?? "CONCLUIDA" : "CONCLUIDA",
                        Observacoes = payload.TryGetProperty("observacoes", out var _c_obsCmp) ? _c_obsCmp.GetString() : null,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Compras.Add(compra);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/compras/{compra.Id}", compra);

                case "guias_entrada":
                case "guias_entradas":
                    var guia = new GuiaEntrada
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_geCod) ? _c_geCod.GetString() : ("GE-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        FornecedorId = payload.TryGetProperty("fornecedor_id", out var _c_fge) && Guid.TryParse(_c_fge.GetString(), out var _c_fgg) ? _c_fgg : null,
                        NumeroGuia = payload.TryGetProperty("numero_guia", out var _c_ng) ? _c_ng.GetString() ?? ("GE-" + DateTime.UtcNow.Year) : ("GE-" + DateTime.UtcNow.Year),
                        DataEmissao = DateTime.UtcNow,
                        ValorTotal = payload.TryGetProperty("valor_total", out var _c_vtg) ? (decimal)_c_vtg.GetDouble() : 0,
                        Status = payload.TryGetProperty("status", out var _c_stGe) ? _c_stGe.GetString() ?? "PROCESSADA" : "PROCESSADA",
                        Observacoes = payload.TryGetProperty("observacoes", out var _c_obsGe) ? _c_obsGe.GetString() : null,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.GuiasEntrada.Add(guia);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/guias_entrada/{guia.Id}", guia);

                case "itens_compra":
                    var itCmp = new ItemCompra
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_itcCod) ? _c_itcCod.GetString() : ("ITC-" + guid.ToString()[..4].ToUpper()),
                        CompraId = payload.TryGetProperty("compra_id", out var _c_cid) && Guid.TryParse(_c_cid.GetString(), out var _c_cg) ? _c_cg : Guid.NewGuid(),
                        ItemStockId = payload.TryGetProperty("item_stock_id", out var _c_isid) && Guid.TryParse(_c_isid.GetString(), out var _c_isg) ? _c_isg : Guid.NewGuid(),
                        Quantidade = payload.TryGetProperty("quantidade", out var _c_qtc) ? (decimal)_c_qtc.GetDouble() : 1,
                        PrecoUnitario = payload.TryGetProperty("preco_unitario", out var _c_puc) ? (decimal)_c_puc.GetDouble() : 0,
                        Subtotal = payload.TryGetProperty("subtotal", out var _c_sbc) ? (decimal)_c_sbc.GetDouble() : 0
                    };
                    _context.ItensCompra.Add(itCmp);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/itens_compra/{itCmp.Id}", itCmp);

                case "itens_guia_entrada":
                case "itens_guias_entrada":
                    var itGe = new ItemGuiaEntrada
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_itgCod) ? _c_itgCod.GetString() : ("ITG-" + guid.ToString()[..4].ToUpper()),
                        GuiaEntradaId = payload.TryGetProperty("guia_entrada_id", out var _c_gid) && Guid.TryParse(_c_gid.GetString(), out var _c_gg) ? _c_gg : Guid.NewGuid(),
                        ItemStockId = payload.TryGetProperty("item_stock_id", out var _c_igid) && Guid.TryParse(_c_igid.GetString(), out var _c_igg) ? _c_igg : Guid.NewGuid(),
                        Quantidade = payload.TryGetProperty("quantidade", out var _c_qtg) ? (decimal)_c_qtg.GetDouble() : 1,
                        PrecoCusto = payload.TryGetProperty("preco_custo", out var _c_pcg) ? (decimal)_c_pcg.GetDouble() : 0,
                        Subtotal = payload.TryGetProperty("subtotal", out var _c_sbg) ? (decimal)_c_sbg.GetDouble() : 0
                    };
                    _context.ItensGuiaEntrada.Add(itGe);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/itens_guia_entrada/{itGe.Id}", itGe);

                case "itens_pedidos_vendas":
                case "itens_pedido_venda":
                    var itPv = new ItemPedidoVenda
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_itpvCod) ? _c_itpvCod.GetString() : ("IPV-" + guid.ToString()[..4].ToUpper()),
                        PedidoId = payload.TryGetProperty("pedido_id", out var _c_pid) && Guid.TryParse(_c_pid.GetString(), out var _c_pg) ? _c_pg : Guid.NewGuid(),
                        ProdutoId = payload.TryGetProperty("produto_id", out var _c_prid) && Guid.TryParse(_c_prid.GetString(), out var _c_prg) ? _c_prg : Guid.NewGuid(),
                        Quantidade = payload.TryGetProperty("quantidade", out var _c_qpv) ? (decimal)_c_qpv.GetDouble() : 1,
                        PrecoUnitario = payload.TryGetProperty("preco_unitario", out var _c_pupv) ? (decimal)_c_pupv.GetDouble() : 0,
                        OpcoesSelecionadas = payload.TryGetProperty("opcoes_selecionadas", out var _c_ops) ? _c_ops.GetString() : null,
                        Subtotal = payload.TryGetProperty("subtotal", out var _c_sbpv) ? (decimal)_c_sbpv.GetDouble() : 0
                    };
                    _context.ItensPedidoVenda.Add(itPv);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/itens_pedidos_vendas/{itPv.Id}", itPv);

                case "locais_armazenamento":
                    var localArm = new LocalArmazenamento
                    {
                        Id = guid,
                        ArmazemId = payload.TryGetProperty("armazem_id", out var _c_arid) && Guid.TryParse(_c_arid.GetString(), out var _c_arg) ? _c_arg : estId,
                        TipoEstrutura = payload.TryGetProperty("tipo_estrutura", out var _c_te) ? _c_te.GetString() ?? "ESTANTE" : "ESTANTE",
                        Codigo = payload.TryGetProperty("codigo", out var _c_locCod) ? _c_locCod.GetString() ?? ("LOC-" + guid.ToString()[..4].ToUpper()) : ("LOC-" + guid.ToString()[..4].ToUpper()),
                        Descricao = payload.TryGetProperty("descricao", out var _c_locDesc) ? _c_locDesc.GetString() : null,
                        Ativo = !payload.TryGetProperty("ativo", out var _c_atLoc) || _c_atLoc.GetBoolean()
                    };
                    _context.LocaisArmazenamento.Add(localArm);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/locais_armazenamento/{localArm.Id}", localArm);

                case "pedidos_online":
                    var pedOnline = new PedidoOnline
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_poCod) ? _c_poCod.GetString() : ("POL-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        PedidoVendaId = payload.TryGetProperty("pedido_venda_id", out var _c_pvi) && Guid.TryParse(_c_pvi.GetString(), out var _c_pvg) ? _c_pvg : null,
                        NumeroPedido = payload.TryGetProperty("numero_pedido", out var _c_numPedOnl) ? _c_numPedOnl.GetString() ?? ("POL-" + DateTime.UtcNow.Ticks.ToString()[..6]) : ("POL-" + DateTime.UtcNow.Ticks.ToString()[..6]),
                        ClienteNome = payload.TryGetProperty("cliente_nome", out var _c_cn) ? _c_cn.GetString() ?? "Cliente Online" : "Cliente Online",
                        ClienteTelefone = payload.TryGetProperty("cliente_telefone", out var _c_ct) ? _c_ct.GetString() ?? "" : "",
                        TipoEntrega = payload.TryGetProperty("tipo_entrega", out var _c_tePol) ? _c_tePol.GetString() ?? "DELIVERY" : "DELIVERY",
                        MesaId = payload.TryGetProperty("mesa_id", out var _c_mid) && Guid.TryParse(_c_mid.GetString(), out var _c_mg) ? _c_mg : null,
                        NumeroMesa = payload.TryGetProperty("numero_mesa", out var _c_nm) ? _c_nm.GetString() : null,
                        EnderecoEntrega = payload.TryGetProperty("endereco_entrega", out var _c_ee) ? _c_ee.GetString() : null,
                        OperadoraPagamento = payload.TryGetProperty("operadora_pagamento", out var _c_op) ? _c_op.GetString() ?? "MPESA" : "MPESA",
                        ReferenciaPagamento = payload.TryGetProperty("referencia_pagamento", out var _c_rp) ? _c_rp.GetString() : null,
                        Subtotal = payload.TryGetProperty("subtotal", out var _c_sbpo) ? (decimal)_c_sbpo.GetDouble() : 0,
                        TaxaEntrega = payload.TryGetProperty("taxa_entrega", out var _c_txpo) ? (decimal)_c_txpo.GetDouble() : 0,
                        Total = payload.TryGetProperty("total", out var _c_totpo) ? (decimal)_c_totpo.GetDouble() : 0,
                        Estado = payload.TryGetProperty("estado", out var _c_stpo) ? _c_stpo.GetString() ?? "PENDENTE_VALIDACAO" : "PENDENTE_VALIDACAO",
                        StockDeduzido = payload.TryGetProperty("stock_deduzido", out var _c_sd) && _c_sd.GetBoolean(),
                        DataPedido = DateTime.UtcNow,
                        ValidadoPor = payload.TryGetProperty("validado_por", out var _c_vp) ? _c_vp.GetString() : null,
                        Observacoes = payload.TryGetProperty("observacoes", out var _c_obspo) ? _c_obspo.GetString() : null
                    };
                    _context.PedidosOnline.Add(pedOnline);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/pedidos_online/{pedOnline.Id}", pedOnline);

                case "pedidos_vendas":
                    var pedVenda = new PedidoVenda
                    {
                        Id = guid,
                        Codigo = payload.TryGetProperty("codigo", out var _c_pvCod) ? _c_pvCod.GetString() : ("PV-" + guid.ToString()[..4].ToUpper()),
                        EstabelecimentoId = estId,
                        CaixaSessaoId = payload.TryGetProperty("caixa_sessao_id", out var _c_csi) && Guid.TryParse(_c_csi.GetString(), out var _c_csg) ? _c_csg : null,
                        ClienteId = payload.TryGetProperty("cliente_id", out var _c_cliId) && Guid.TryParse(_c_cliId.GetString(), out var _c_clig) ? _c_clig : null,
                        SalaId = payload.TryGetProperty("sala_id", out var _c_sId) && Guid.TryParse(_c_sId.GetString(), out var _c_sg) ? _c_sg : null,
                        MesaId = payload.TryGetProperty("mesa_id", out var _c_mId) && Guid.TryParse(_c_mId.GetString(), out var _c_mggg) ? _c_mggg : null,
                        NumeroMesa = payload.TryGetProperty("numero_mesa", out var _c_numM) ? _c_numM.GetString() : null,
                        TipoOrigem = payload.TryGetProperty("tipo_origem", out var _c_to) ? _c_to.GetString() ?? "PDV_BALCAO" : "PDV_BALCAO",
                        NumeroPedido = payload.TryGetProperty("numero_pedido", out var _c_numPed) ? _c_numPed.GetString() ?? ("VND-" + DateTime.UtcNow.Ticks.ToString()[..6]) : ("VND-" + DateTime.UtcNow.Ticks.ToString()[..6]),
                        ClienteNome = payload.TryGetProperty("cliente_nome", out var _c_cnpv) ? _c_cnpv.GetString() : null,
                        ClienteTelefone = payload.TryGetProperty("cliente_telefone", out var _c_ctpv) ? _c_ctpv.GetString() : null,
                        EnderecoEntrega = payload.TryGetProperty("endereco_entrega", out var _c_eepv) ? _c_eepv.GetString() : null,
                        MetodoPagamento = payload.TryGetProperty("metodo_pagamento", out var _c_mppv) ? _c_mppv.GetString() ?? "DINHEIRO" : "DINHEIRO",
                        Subtotal = payload.TryGetProperty("subtotal", out var _c_sbpvv) ? (decimal)_c_sbpvv.GetDouble() : 0,
                        TaxaEntrega = payload.TryGetProperty("taxa_entrega", out var _c_txpv) ? (decimal)_c_txpv.GetDouble() : 0,
                        Desconto = payload.TryGetProperty("desconto", out var _c_dsc) ? (decimal)_c_dsc.GetDouble() : 0,
                        ValorTotal = payload.TryGetProperty("valor_total", out var _c_vtpv) ? (decimal)_c_vtpv.GetDouble() : 0,
                        Estado = payload.TryGetProperty("estado", out var _c_stpv) ? _c_stpv.GetString() ?? "PENDENTE" : "PENDENTE",
                        Observacoes = payload.TryGetProperty("observacoes", out var _c_obspv) ? _c_obspv.GetString() : null,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.PedidosVendas.Add(pedVenda);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/pedidos_vendas/{pedVenda.Id}", pedVenda);

                case "unidades_medida":
                case "unidades_medidas":
                    var unid = new UnidadeMedida
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        Codigo = payload.TryGetProperty("codigo", out var _c_uCod) ? _c_uCod.GetString() ?? ("UN-" + guid.ToString()[..4].ToUpper()) : ("UN-" + guid.ToString()[..4].ToUpper()),
                        Nome = payload.TryGetProperty("nome", out var _c_uNome) ? _c_uNome.GetString() ?? "Unidade" : "Unidade",
                        PermiteDecimal = payload.TryGetProperty("permite_decimal", out var _c_pdUm) && _c_pdUm.GetBoolean(),
                        Ativo = !payload.TryGetProperty("ativo", out var _c_atUm) || _c_atUm.GetBoolean()
                    };
                    _context.UnidadesMedida.Add(unid);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/unidades_medida/{unid.Id}", unid);

                case "tabela_precos_fornecedores":
                case "tabela_precos":
                case "precos_fornecedores":
                    var tpNovo = new TabelaPrecoFornecedor
                    {
                        Id = guid,
                        EstabelecimentoId = estId,
                        ProdutoId = payload.TryGetProperty("produto_id", out var _c_tppid) ? _c_tppid.GetString() : null,
                        ProdutoNome = payload.TryGetProperty("produto_nome", out var _c_tppn) ? _c_tppn.GetString() ?? "" : "",
                        FornecedorId = payload.TryGetProperty("fornecedor_id", out var _c_tpfid) ? _c_tpfid.GetString() : null,
                        FornecedorNome = payload.TryGetProperty("fornecedor_nome", out var _c_tpfn) ? _c_tpfn.GetString() ?? "" : "",
                        CustoAtual = payload.TryGetProperty("custo_atual", out var _c_tpca) ? (decimal)_c_tpca.GetDouble() : 0m,
                        CustoAtualStock = payload.TryGetProperty("custo_atual_stock", out var _c_tpcas) ? (decimal)_c_tpcas.GetDouble() : 0m,
                        PrecoCaixa = payload.TryGetProperty("preco_caixa", out var _c_tppc) ? (decimal)_c_tppc.GetDouble() : 0m,
                        UnidadesCaixa = payload.TryGetProperty("unidades_caixa", out var _c_tpuc) ? _c_tpuc.GetInt32() : 1,
                        UnidadesPorCaixa = payload.TryGetProperty("unidades_por_caixa", out var _c_tpupc) ? _c_tpupc.GetInt32() : 1,
                        CustoUnitario = payload.TryGetProperty("custo_unitario", out var _c_tpcu) ? (decimal)_c_tpcu.GetDouble() : 0m,
                        CustoUnitarioCalculado = payload.TryGetProperty("custo_unitario_calculado", out var _c_tpcuc) ? (decimal)_c_tpcuc.GetDouble() : 0m,
                        PrecoVendaSugerido = payload.TryGetProperty("preco_venda_sugerido", out var _c_tppvs) ? (decimal)_c_tppvs.GetDouble() : 0m,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.TabelaPrecosFornecedores.Add(tpNovo);
                    await _context.SaveChangesAsync();
                    return Created($"/api/crud/tabela_precos_fornecedores/{tpNovo.Id}", tpNovo);

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
                    if (payload.TryGetProperty("nome", out var nc))
                    {
                        var novoNome = nc.GetString()?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(novoNome))
                        {
                            return BadRequest(new { erro = "O nome da categoria não pode ficar vazio." });
                        }
                        var existeOutra = await _context.Categorias.AsNoTracking().AnyAsync(c => c.Id != guid && c.Nome.ToLower() == novoNome.ToLower());
                        if (existeOutra)
                        {
                            return BadRequest(new { erro = $"Já existe outra categoria registada com o nome '{novoNome}'." });
                        }
                        cat.Nome = novoNome;
                    }
                    if (payload.TryGetProperty("codigo", out var cc)) cat.Codigo = cc.GetString() ?? cat.Codigo;
                    if (payload.TryGetProperty("emoji", out var em)) cat.Emoji = em.GetString() ?? cat.Emoji;
                    if (payload.TryGetProperty("ordem", out var ord)) cat.Ordem = ord.GetInt32();
                    await _context.SaveChangesAsync();
                    return Ok(cat);

                case "clientes":
                    var cli = await _context.Clientes.FindAsync(guid);
                    if (cli == null) return NotFound(new { erro = "Cliente não encontrado." });
                    if (payload.TryGetProperty("email", out var ecli))
                    {
                        var emailStr = ecli.GetString()?.Trim();
                        var errEmail = ValidarEmail(emailStr, obrigatorio: false);
                        if (errEmail != null) return BadRequest(new { erro = errEmail });
                        cli.Email = emailStr;
                    }
                    if (payload.TryGetProperty("telefone", out var tcli))
                    {
                        var telStr = tcli.GetString()?.Trim();
                        var errTel = ValidarTelefone(telStr, obrigatorio: false);
                        if (errTel != null) return BadRequest(new { erro = errTel });
                        cli.Telefone = telStr ?? cli.Telefone;
                    }
                    if (payload.TryGetProperty("nuit", out var n1))
                    {
                        var nuitStr = n1.GetString()?.Trim();
                        var errNuit = ValidarNuit(nuitStr, obrigatorio: false);
                        if (errNuit != null) return BadRequest(new { erro = errNuit });
                        cli.Nif = nuitStr;
                    }
                    else if (payload.TryGetProperty("nif", out var n2))
                    {
                        var nifStr = n2.GetString()?.Trim();
                        var errNif = ValidarNuit(nifStr, obrigatorio: false);
                        if (errNif != null) return BadRequest(new { erro = errNif });
                        cli.Nif = nifStr;
                    }
                    if (payload.TryGetProperty("codigo", out var codCli)) cli.Codigo = codCli.GetString() ?? cli.Codigo;
                    if (payload.TryGetProperty("nome", out var ncli)) cli.Nome = ncli.GetString() ?? cli.Nome;
                    if (payload.TryGetProperty("morada", out var m1)) cli.Morada = m1.GetString();
                    else if (payload.TryGetProperty("endereco", out var m2)) cli.Morada = m2.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(cli);

                case "fornecedores":
                    var forn = await _context.Fornecedores.FindAsync(guid);
                    if (forn == null) return NotFound(new { erro = "Fornecedor não encontrado." });
                    if (payload.TryGetProperty("email", out var eforn))
                    {
                        var emailStr = eforn.GetString()?.Trim();
                        var errEmail = ValidarEmail(emailStr, obrigatorio: false);
                        if (errEmail != null) return BadRequest(new { erro = errEmail });
                        forn.Email = emailStr;
                    }
                    if (payload.TryGetProperty("telefone", out var tforn))
                    {
                        var telStr = tforn.GetString()?.Trim();
                        var errTel = ValidarTelefone(telStr, obrigatorio: false);
                        if (errTel != null) return BadRequest(new { erro = errTel });
                        forn.Telefone = telStr;
                    }
                    if (payload.TryGetProperty("nuit", out var fn1))
                    {
                        var nuitStr = fn1.GetString()?.Trim();
                        var errNuit = ValidarNuit(nuitStr, obrigatorio: false);
                        if (errNuit != null) return BadRequest(new { erro = errNuit });
                        forn.Nif = nuitStr;
                    }
                    else if (payload.TryGetProperty("nif", out var fn2))
                    {
                        var nifStr = fn2.GetString()?.Trim();
                        var errNif = ValidarNuit(nifStr, obrigatorio: false);
                        if (errNif != null) return BadRequest(new { erro = errNif });
                        forn.Nif = nifStr;
                    }
                    if (payload.TryGetProperty("codigo", out var codForn)) forn.Codigo = codForn.GetString() ?? forn.Codigo;
                    if (payload.TryGetProperty("nome", out var nforn)) forn.Nome = nforn.GetString() ?? forn.Nome;
                    if (payload.TryGetProperty("contacto", out var cr)) forn.ContactoResponsavel = cr.GetString();
                    if (payload.TryGetProperty("morada", out var mf)) forn.Morada = mf.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(forn);

                case "utilizadores":
                    var user = await _context.Utilizadores.FindAsync(guid);
                    if (user == null) return NotFound(new { erro = "Utilizador não encontrado." });
                    if (payload.TryGetProperty("email", out var eu))
                    {
                        var emailStr = eu.GetString()?.Trim();
                        var errEmail = ValidarEmail(emailStr, obrigatorio: true);
                        if (errEmail != null) return BadRequest(new { erro = errEmail });
                        user.Email = emailStr!;
                    }
                    if (payload.TryGetProperty("codigo", out var codUser)) user.Codigo = codUser.GetString() ?? user.Codigo;
                    if (payload.TryGetProperty("nome", out var nu)) user.Nome = nu.GetString() ?? user.Nome;
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

                case "postos_preparacao":
                    var posto = await _context.PostosPreparacao.FindAsync(guid);
                    if (posto == null) return NotFound(new { erro = "Posto de preparação não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_ppc)) posto.Codigo = _u_ppc.GetString() ?? posto.Codigo;
                    if (payload.TryGetProperty("descricao", out var _u_ppd)) posto.Descricao = _u_ppd.GetString() ?? posto.Descricao;
                    else if (payload.TryGetProperty("nome", out var _u_ppn)) posto.Descricao = _u_ppn.GetString() ?? posto.Descricao;
                    if (payload.TryGetProperty("ativo", out var _u_ppa)) posto.Ativo = _u_ppa.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(posto);

                case "tipos_pedidos":
                    var tipoPed = await _context.TiposPedidos.FindAsync(guid);
                    if (tipoPed == null) return NotFound(new { erro = "Tipo de pedido não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_tpc)) tipoPed.Codigo = _u_tpc.GetString() ?? tipoPed.Codigo;
                    if (payload.TryGetProperty("descricao", out var _u_tpd)) tipoPed.Descricao = _u_tpd.GetString() ?? tipoPed.Descricao;
                    else if (payload.TryGetProperty("nome", out var _u_tpn)) tipoPed.Descricao = _u_tpn.GetString() ?? tipoPed.Descricao;
                    if (payload.TryGetProperty("ativo", out var _u_tpa)) tipoPed.Ativo = _u_tpa.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(tipoPed);

                case "metodos_pagamento":
                    var metPag = await _context.MetodosPagamento.FindAsync(guid);
                    if (metPag == null) return NotFound(new { erro = "Método de pagamento não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_mpc)) metPag.Codigo = _u_mpc.GetString() ?? metPag.Codigo;
                    if (payload.TryGetProperty("nome", out var _u_mpn)) metPag.Nome = _u_mpn.GetString() ?? metPag.Nome;
                    if (payload.TryGetProperty("icone", out var _u_mpi)) metPag.Icone = _u_mpi.GetString() ?? metPag.Icone;
                    if (payload.TryGetProperty("taxa_percentual", out var _u_mptx)) metPag.TaxaPercentual = (decimal)_u_mptx.GetDouble();
                    if (payload.TryGetProperty("permite_troco", out var _u_mppt)) metPag.PermiteTroco = _u_mppt.GetBoolean();
                    if (payload.TryGetProperty("ordem", out var _u_mpo)) metPag.Ordem = _u_mpo.GetInt32();
                    if (payload.TryGetProperty("ativo", out var _u_mpa)) metPag.Ativo = _u_mpa.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(metPag);

                case "acompanhamentos":
                    var acomp = await _context.Acompanhamentos.FindAsync(guid);
                    if (acomp == null) return NotFound(new { erro = "Acompanhamento não encontrado." });
                    if (payload.TryGetProperty("nome", out var _u_acn)) acomp.Nome = _u_acn.GetString() ?? acomp.Nome;
                    if (payload.TryGetProperty("tipo", out var _u_act)) acomp.TipoSelecao = _u_act.GetString() ?? acomp.TipoSelecao;
                    if (payload.TryGetProperty("preco_adicional", out var _u_acpa)) acomp.PrecoAdicional = (decimal)_u_acpa.GetDouble();
                    if (payload.TryGetProperty("disponivel", out var _u_acd)) acomp.Disponivel = _u_acd.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(acomp);

                case "armazens":
                    var arm = await _context.Armazens.FindAsync(guid);
                    if (arm == null) return NotFound(new { erro = "Armazém não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_armC)) arm.Codigo = _u_armC.GetString() ?? arm.Codigo;
                    if (payload.TryGetProperty("nome", out var _u_armN)) arm.Nome = _u_armN.GetString() ?? arm.Nome;
                    if (payload.TryGetProperty("tipo", out var _u_armT)) arm.Tipo = _u_armT.GetString() ?? arm.Tipo;
                    if (payload.TryGetProperty("ativo", out var _u_armA)) arm.Ativo = _u_armA.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(arm);

                case "caixas_sessoes":
                case "caixa_sessoes":
                    var cs = await _context.CaixasSessoes.FindAsync(guid);
                    if (cs == null) return NotFound(new { erro = "Sessão de caixa não encontrada." });
                    if (payload.TryGetProperty("codigo", out var _u_csC)) cs.Codigo = _u_csC.GetString() ?? cs.Codigo;
                    if (payload.TryGetProperty("saldo_fecho", out var _u_sf)) cs.SaldoFecho = (decimal)_u_sf.GetDouble();
                    if (payload.TryGetProperty("estado", out var _u_csEst)) cs.Estado = _u_csEst.GetString() ?? cs.Estado;
                    if (payload.TryGetProperty("observacoes", out var _u_csObs)) cs.Observacoes = _u_csObs.GetString();
                    if (cs.Estado == "FECHADO" && cs.FechadoEm == null) cs.FechadoEm = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok(cs);

                case "canais_pagamento":
                    var canalPag = await _context.CanaisPagamento.FindAsync(guid);
                    if (canalPag == null) return NotFound(new { erro = "Canal de pagamento não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_cpC)) canalPag.Codigo = _u_cpC.GetString() ?? canalPag.Codigo;
                    if (payload.TryGetProperty("codigo_operadora", out var _u_cpOp)) canalPag.CodigoOperadora = _u_cpOp.GetString() ?? canalPag.CodigoOperadora;
                    if (payload.TryGetProperty("descricao", out var _u_cpD)) canalPag.Descricao = _u_cpD.GetString() ?? canalPag.Descricao;
                    if (payload.TryGetProperty("numero_associado", out var _u_cpN)) canalPag.NumeroAssociado = _u_cpN.GetString() ?? canalPag.NumeroAssociado;
                    if (payload.TryGetProperty("titular_conta", out var _u_cpTc)) canalPag.TitularConta = _u_cpTc.GetString();
                    if (payload.TryGetProperty("instrucoes_pagamento", out var _u_cpIp)) canalPag.InstrucoesPagamento = _u_cpIp.GetString();
                    if (payload.TryGetProperty("ativo", out var _u_cpAt)) canalPag.Ativo = _u_cpAt.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(canalPag);

                case "categorias_stock":
                    var cstk = await _context.CategoriasStock.FindAsync(guid);
                    if (cstk == null) return NotFound(new { erro = "Categoria de stock não encontrada." });
                    if (payload.TryGetProperty("codigo", out var _u_cstkC)) cstk.Codigo = _u_cstkC.GetString() ?? cstk.Codigo;
                    if (payload.TryGetProperty("nome", out var _u_cstkN)) cstk.Nome = _u_cstkN.GetString() ?? cstk.Nome;
                    if (payload.TryGetProperty("descricao", out var _u_cstkD)) cstk.Descricao = _u_cstkD.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(cstk);

                case "compras":
                    var cmp = await _context.Compras.FindAsync(guid);
                    if (cmp == null) return NotFound(new { erro = "Compra não encontrada." });
                    if (payload.TryGetProperty("codigo", out var _u_cmpC)) cmp.Codigo = _u_cmpC.GetString() ?? cmp.Codigo;
                    if (payload.TryGetProperty("numero_fatura", out var _u_cmpNf)) cmp.NumeroFatura = _u_cmpNf.GetString();
                    if (payload.TryGetProperty("valor_total", out var _u_cmpVt)) cmp.ValorTotal = (decimal)_u_cmpVt.GetDouble();
                    if (payload.TryGetProperty("status", out var _u_cmpSt)) cmp.Status = _u_cmpSt.GetString() ?? cmp.Status;
                    if (payload.TryGetProperty("observacoes", out var _u_cmpObs)) cmp.Observacoes = _u_cmpObs.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(cmp);

                case "guias_entrada":
                case "guias_entradas":
                    var ge = await _context.GuiasEntrada.FindAsync(guid);
                    if (ge == null) return NotFound(new { erro = "Guia de entrada não encontrada." });
                    if (payload.TryGetProperty("codigo", out var _u_geC)) ge.Codigo = _u_geC.GetString() ?? ge.Codigo;
                    if (payload.TryGetProperty("numero_guia", out var _u_geNg)) ge.NumeroGuia = _u_geNg.GetString() ?? ge.NumeroGuia;
                    if (payload.TryGetProperty("valor_total", out var _u_geVt)) ge.ValorTotal = (decimal)_u_geVt.GetDouble();
                    if (payload.TryGetProperty("status", out var _u_geSt)) ge.Status = _u_geSt.GetString() ?? ge.Status;
                    if (payload.TryGetProperty("observacoes", out var _u_geObs)) ge.Observacoes = _u_geObs.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(ge);

                case "itens_compra":
                    var icmp = await _context.ItensCompra.FindAsync(guid);
                    if (icmp == null) return NotFound(new { erro = "Item de compra não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_icC)) icmp.Codigo = _u_icC.GetString() ?? icmp.Codigo;
                    if (payload.TryGetProperty("quantidade", out var _u_icQ)) icmp.Quantidade = (decimal)_u_icQ.GetDouble();
                    if (payload.TryGetProperty("preco_unitario", out var _u_icPu)) icmp.PrecoUnitario = (decimal)_u_icPu.GetDouble();
                    if (payload.TryGetProperty("subtotal", out var _u_icSb)) icmp.Subtotal = (decimal)_u_icSb.GetDouble();
                    await _context.SaveChangesAsync();
                    return Ok(icmp);

                case "itens_guia_entrada":
                case "itens_guias_entrada":
                    var ige = await _context.ItensGuiaEntrada.FindAsync(guid);
                    if (ige == null) return NotFound(new { erro = "Item de guia de entrada não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_igC)) ige.Codigo = _u_igC.GetString() ?? ige.Codigo;
                    if (payload.TryGetProperty("quantidade", out var _u_igQ)) ige.Quantidade = (decimal)_u_igQ.GetDouble();
                    if (payload.TryGetProperty("preco_custo", out var _u_igPc)) ige.PrecoCusto = (decimal)_u_igPc.GetDouble();
                    if (payload.TryGetProperty("subtotal", out var _u_igSb)) ige.Subtotal = (decimal)_u_igSb.GetDouble();
                    await _context.SaveChangesAsync();
                    return Ok(ige);

                case "itens_pedidos_vendas":
                case "itens_pedido_venda":
                    var ipv = await _context.ItensPedidoVenda.FindAsync(guid);
                    if (ipv == null) return NotFound(new { erro = "Item de pedido de venda não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_ipvC)) ipv.Codigo = _u_ipvC.GetString() ?? ipv.Codigo;
                    if (payload.TryGetProperty("quantidade", out var _u_ipvQ)) ipv.Quantidade = (decimal)_u_ipvQ.GetDouble();
                    if (payload.TryGetProperty("preco_unitario", out var _u_ipvPu)) ipv.PrecoUnitario = (decimal)_u_ipvPu.GetDouble();
                    if (payload.TryGetProperty("opcoes_selecionadas", out var _u_ipvOps)) ipv.OpcoesSelecionadas = _u_ipvOps.GetString();
                    if (payload.TryGetProperty("subtotal", out var _u_ipvSb)) ipv.Subtotal = (decimal)_u_ipvSb.GetDouble();
                    await _context.SaveChangesAsync();
                    return Ok(ipv);

                case "locais_armazenamento":
                    var loc = await _context.LocaisArmazenamento.FindAsync(guid);
                    if (loc == null) return NotFound(new { erro = "Local de armazenamento não encontrado." });
                    if (payload.TryGetProperty("tipo_estrutura", out var _u_locTe)) loc.TipoEstrutura = _u_locTe.GetString() ?? loc.TipoEstrutura;
                    if (payload.TryGetProperty("codigo", out var _u_locC)) loc.Codigo = _u_locC.GetString() ?? loc.Codigo;
                    if (payload.TryGetProperty("descricao", out var _u_locD)) loc.Descricao = _u_locD.GetString();
                    if (payload.TryGetProperty("ativo", out var _u_locAt)) loc.Ativo = _u_locAt.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(loc);

                case "pedidos_online":
                    var po = await _context.PedidosOnline.FindAsync(guid);
                    if (po == null) return NotFound(new { erro = "Pedido online não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_poC)) po.Codigo = _u_poC.GetString() ?? po.Codigo;
                    if (payload.TryGetProperty("cliente_nome", out var _u_poCn)) po.ClienteNome = _u_poCn.GetString() ?? po.ClienteNome;
                    if (payload.TryGetProperty("cliente_telefone", out var _u_poCt)) po.ClienteTelefone = _u_poCt.GetString() ?? po.ClienteTelefone;
                    if (payload.TryGetProperty("tipo_entrega", out var _u_poTe)) po.TipoEntrega = _u_poTe.GetString() ?? po.TipoEntrega;
                    if (payload.TryGetProperty("numero_mesa", out var _u_poNm)) po.NumeroMesa = _u_poNm.GetString();
                    if (payload.TryGetProperty("endereco_entrega", out var _u_poEe)) po.EnderecoEntrega = _u_poEe.GetString();
                    if (payload.TryGetProperty("operadora_pagamento", out var _u_poOp)) po.OperadoraPagamento = _u_poOp.GetString() ?? po.OperadoraPagamento;
                    if (payload.TryGetProperty("referencia_pagamento", out var _u_poRp)) po.ReferenciaPagamento = _u_poRp.GetString();
                    if (payload.TryGetProperty("subtotal", out var _u_poSb)) po.Subtotal = (decimal)_u_poSb.GetDouble();
                    if (payload.TryGetProperty("taxa_entrega", out var _u_poTx)) po.TaxaEntrega = (decimal)_u_poTx.GetDouble();
                    if (payload.TryGetProperty("total", out var _u_poTot)) po.Total = (decimal)_u_poTot.GetDouble();
                    if (payload.TryGetProperty("estado", out var _u_poSt)) po.Estado = _u_poSt.GetString() ?? po.Estado;
                    if (payload.TryGetProperty("stock_deduzido", out var _u_poSd)) po.StockDeduzido = _u_poSd.GetBoolean();
                    if (payload.TryGetProperty("validado_por", out var _u_poVp)) po.ValidadoPor = _u_poVp.GetString();
                    if (payload.TryGetProperty("observacoes", out var _u_poObs)) po.Observacoes = _u_poObs.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(po);

                case "pedidos_vendas":
                    var pv = await _context.PedidosVendas.FindAsync(guid);
                    if (pv == null) return NotFound(new { erro = "Pedido de venda não encontrado." });
                    if (payload.TryGetProperty("codigo", out var _u_pvC)) pv.Codigo = _u_pvC.GetString() ?? pv.Codigo;
                    if (payload.TryGetProperty("numero_mesa", out var _u_pvNm)) pv.NumeroMesa = _u_pvNm.GetString();
                    if (payload.TryGetProperty("cliente_nome", out var _u_pvCn)) pv.ClienteNome = _u_pvCn.GetString();
                    if (payload.TryGetProperty("cliente_telefone", out var _u_pvCt)) pv.ClienteTelefone = _u_pvCt.GetString();
                    if (payload.TryGetProperty("metodo_pagamento", out var _u_pvMp)) pv.MetodoPagamento = _u_pvMp.GetString() ?? pv.MetodoPagamento;
                    if (payload.TryGetProperty("subtotal", out var _u_pvSb)) pv.Subtotal = (decimal)_u_pvSb.GetDouble();
                    if (payload.TryGetProperty("taxa_entrega", out var _u_pvTx)) pv.TaxaEntrega = (decimal)_u_pvTx.GetDouble();
                    if (payload.TryGetProperty("desconto", out var _u_pvDsc)) pv.Desconto = (decimal)_u_pvDsc.GetDouble();
                    if (payload.TryGetProperty("valor_total", out var _u_pvVt)) pv.ValorTotal = (decimal)_u_pvVt.GetDouble();
                    if (payload.TryGetProperty("estado", out var _u_pvSt)) pv.Estado = _u_pvSt.GetString() ?? pv.Estado;
                    if (payload.TryGetProperty("observacoes", out var _u_pvObs)) pv.Observacoes = _u_pvObs.GetString();
                    await _context.SaveChangesAsync();
                    return Ok(pv);

                case "unidades_medida":
                case "unidades_medidas":
                    var um = await _context.UnidadesMedida.FindAsync(guid);
                    if (um == null) return NotFound(new { erro = "Unidade de medida não encontrada." });
                    if (payload.TryGetProperty("codigo", out var _u_umC)) um.Codigo = _u_umC.GetString() ?? um.Codigo;
                    if (payload.TryGetProperty("nome", out var _u_umN)) um.Nome = _u_umN.GetString() ?? um.Nome;
                    if (payload.TryGetProperty("permite_decimal", out var _u_umPd)) um.PermiteDecimal = _u_umPd.GetBoolean();
                    if (payload.TryGetProperty("ativo", out var _u_umAt)) um.Ativo = _u_umAt.GetBoolean();
                    await _context.SaveChangesAsync();
                    return Ok(um);

                case "tabela_precos_fornecedores":
                case "tabela_precos":
                case "precos_fornecedores":
                    var tpEdit = await _context.TabelaPrecosFornecedores.FindAsync(guid);
                    if (tpEdit == null) return NotFound(new { erro = "Registo da tabela de preços não encontrado." });
                    if (payload.TryGetProperty("produto_id", out var _u_tppid)) tpEdit.ProdutoId = _u_tppid.GetString();
                    if (payload.TryGetProperty("produto_nome", out var _u_tppn)) tpEdit.ProdutoNome = _u_tppn.GetString() ?? tpEdit.ProdutoNome;
                    if (payload.TryGetProperty("fornecedor_id", out var _u_tpfid)) tpEdit.FornecedorId = _u_tpfid.GetString();
                    if (payload.TryGetProperty("fornecedor_nome", out var _u_tpfn)) tpEdit.FornecedorNome = _u_tpfn.GetString() ?? tpEdit.FornecedorNome;
                    if (payload.TryGetProperty("custo_atual", out var _u_tpca)) tpEdit.CustoAtual = (decimal)_u_tpca.GetDouble();
                    if (payload.TryGetProperty("custo_atual_stock", out var _u_tpcas)) tpEdit.CustoAtualStock = (decimal)_u_tpcas.GetDouble();
                    if (payload.TryGetProperty("preco_caixa", out var _u_tppc)) tpEdit.PrecoCaixa = (decimal)_u_tppc.GetDouble();
                    if (payload.TryGetProperty("unidades_caixa", out var _u_tpuc)) tpEdit.UnidadesCaixa = _u_tpuc.GetInt32();
                    if (payload.TryGetProperty("unidades_por_caixa", out var _u_tpupc)) tpEdit.UnidadesPorCaixa = _u_tpupc.GetInt32();
                    if (payload.TryGetProperty("custo_unitario", out var _u_tpcu)) tpEdit.CustoUnitario = (decimal)_u_tpcu.GetDouble();
                    if (payload.TryGetProperty("custo_unitario_calculado", out var _u_tpcuc)) tpEdit.CustoUnitarioCalculado = (decimal)_u_tpcuc.GetDouble();
                    if (payload.TryGetProperty("preco_venda_sugerido", out var _u_tppvs)) tpEdit.PrecoVendaSugerido = (decimal)_u_tppvs.GetDouble();
                    await _context.SaveChangesAsync();
                    return Ok(tpEdit);

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

                case "postos_preparacao":
                    var postoDel = await _context.PostosPreparacao.FindAsync(guid);
                    if (postoDel == null) return NotFound(new { erro = "Posto de preparação não encontrado." });
                    _context.PostosPreparacao.Remove(postoDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Posto '{postoDel.Descricao}' eliminado com sucesso." });

                case "tipos_pedidos":
                    var tpDel = await _context.TiposPedidos.FindAsync(guid);
                    if (tpDel == null) return NotFound(new { erro = "Tipo de pedido não encontrado." });
                    _context.TiposPedidos.Remove(tpDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Tipo de pedido '{tpDel.Descricao}' eliminado com sucesso." });

                case "armazens":
                    var armDel = await _context.Armazens.FindAsync(guid);
                    if (armDel == null) return NotFound(new { erro = "Armazém não encontrado." });
                    _context.Armazens.Remove(armDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Armazém '{armDel.Nome}' eliminado com sucesso." });

                case "caixas_sessoes":
                case "caixa_sessoes":
                    var csDel = await _context.CaixasSessoes.FindAsync(guid);
                    if (csDel == null) return NotFound(new { erro = "Sessão de caixa não encontrada." });
                    _context.CaixasSessoes.Remove(csDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = "Sessão de caixa eliminada com sucesso." });

                case "canais_pagamento":
                    var cpDel = await _context.CanaisPagamento.FindAsync(guid);
                    if (cpDel == null) return NotFound(new { erro = "Canal de pagamento não encontrado." });
                    _context.CanaisPagamento.Remove(cpDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Canal '{cpDel.Descricao}' eliminado com sucesso." });

                case "categorias_stock":
                    var cstkDel = await _context.CategoriasStock.FindAsync(guid);
                    if (cstkDel == null) return NotFound(new { erro = "Categoria de stock não encontrada." });
                    _context.CategoriasStock.Remove(cstkDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Categoria de stock '{cstkDel.Nome}' eliminada com sucesso." });

                case "compras":
                    var cmpDel = await _context.Compras.FindAsync(guid);
                    if (cmpDel == null) return NotFound(new { erro = "Compra não encontrada." });
                    _context.Compras.Remove(cmpDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = "Compra eliminada com sucesso." });

                case "guias_entrada":
                case "guias_entradas":
                    var geDel = await _context.GuiasEntrada.FindAsync(guid);
                    if (geDel == null) return NotFound(new { erro = "Guia de entrada não encontrada." });
                    _context.GuiasEntrada.Remove(geDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Guia '{geDel.NumeroGuia}' eliminada com sucesso." });

                case "itens_compra":
                    var icDel = await _context.ItensCompra.FindAsync(guid);
                    if (icDel == null) return NotFound(new { erro = "Item de compra não encontrado." });
                    _context.ItensCompra.Remove(icDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = "Item de compra eliminado com sucesso." });

                case "itens_guia_entrada":
                case "itens_guias_entrada":
                    var igDel = await _context.ItensGuiaEntrada.FindAsync(guid);
                    if (igDel == null) return NotFound(new { erro = "Item de guia não encontrado." });
                    _context.ItensGuiaEntrada.Remove(igDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = "Item de guia eliminado com sucesso." });

                case "itens_pedidos_vendas":
                case "itens_pedido_venda":
                    var ipvDel = await _context.ItensPedidoVenda.FindAsync(guid);
                    if (ipvDel == null) return NotFound(new { erro = "Item de pedido não encontrado." });
                    _context.ItensPedidoVenda.Remove(ipvDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = "Item de pedido de venda eliminado com sucesso." });

                case "locais_armazenamento":
                    var locDel = await _context.LocaisArmazenamento.FindAsync(guid);
                    if (locDel == null) return NotFound(new { erro = "Local de armazenamento não encontrado." });
                    _context.LocaisArmazenamento.Remove(locDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Local '{locDel.Descricao ?? locDel.Codigo}' eliminado com sucesso." });

                case "pedidos_online":
                    var poDel = await _context.PedidosOnline.FindAsync(guid);
                    if (poDel == null) return NotFound(new { erro = "Pedido online não encontrado." });
                    _context.PedidosOnline.Remove(poDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Pedido online '{poDel.NumeroPedido}' eliminado com sucesso." });

                case "pedidos_vendas":
                    var pvDel = await _context.PedidosVendas.FindAsync(guid);
                    if (pvDel == null) return NotFound(new { erro = "Pedido de venda não encontrado." });
                    _context.PedidosVendas.Remove(pvDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Pedido de venda '{pvDel.NumeroPedido}' eliminado com sucesso." });

                case "unidades_medida":
                case "unidades_medidas":
                    var umDel = await _context.UnidadesMedida.FindAsync(guid);
                    if (umDel == null) return NotFound(new { erro = "Unidade de medida não encontrada." });
                    _context.UnidadesMedida.Remove(umDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Unidade de medida '{umDel.Nome}' eliminada com sucesso." });

                case "tabela_precos_fornecedores":
                case "tabela_precos":
                case "precos_fornecedores":
                    var tpfDel = await _context.TabelaPrecosFornecedores.FindAsync(guid);
                    if (tpfDel == null) return NotFound(new { erro = "Registo da tabela de preços não encontrado." });
                    _context.TabelaPrecosFornecedores.Remove(tpfDel);
                    await _context.SaveChangesAsync();
                    return Ok(new { status = "success", message = $"Preço de fornecedor '{tpfDel.ProdutoNome}' eliminado com sucesso." });

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
