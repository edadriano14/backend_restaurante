using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncStateController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<SyncStateController> _logger;

    public SyncStateController(AppDbContext context, ITenantService tenantService, ILogger<SyncStateController> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }

    public static Guid ToGuid(string? id, string namespacePrefix = "default")
    {
        if (string.IsNullOrWhiteSpace(id)) return Guid.NewGuid();
        if (Guid.TryParse(id, out var parsed)) return parsed;

        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes($"{namespacePrefix}:{id.Trim()}"));
        return new Guid(hash);
    }

    [HttpGet("state")]
    public async Task<IActionResult> ObterEstadoGlobal()
    {
        try
        {
            var estabelecimentos = await _context.Estabelecimentos.AsNoTracking().ToListAsync();

            if (!estabelecimentos.Any())
            {
                var estPadrao = new Estabelecimento
                {
                    Id = ToGuid("1", "est"),
                    Nome = "Restaurante Sabor & Brasa",
                    Slug = "sabor-brasa",
                    Moeda = "MZN",
                    CorPrimaria = "#DC2626",
                    CorSecundaria = "#1E293B",
                    LogotipoUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=150&auto=format&fit=crop&q=80",
                    Telefone = "+258 84 000 0000",
                    Email = "contacto@saborbrasa.co.mz",
                    Endereco = "Av. Julius Nyerere, Maputo",
                    Cidade = "Maputo",
                    HorarioAbertura = "10:00 - 23:00",
                    DiasFuncionamento = "Segunda a Domingo",
                    Whatsapp = "+258840000000",
                    Facebook = "https://facebook.com/saborbrasa",
                    Instagram = "https://instagram.com/saborbrasa",
                    Website = "https://saborbrasa.co.mz",
                    DescricaoRodape = "O autêntico sabor das melhores carnes e iguarias moçambicanas no coração de Maputo.",
                    TemPiscina = true,
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Estabelecimentos.Add(estPadrao);
                await _context.SaveChangesAsync();
                estabelecimentos.Add(estPadrao);
            }

            var utilizadores = await _context.Utilizadores.AsNoTracking().ToListAsync();
            var fornecedores = await _context.Fornecedores.AsNoTracking().ToListAsync();
            var categorias = await _context.Categorias.AsNoTracking().OrderBy(c => c.Ordem).ToListAsync();
            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
            var salas = await _context.Salas.AsNoTracking().ToListAsync();
            var mesas = await _context.Mesas.AsNoTracking().ToListAsync();
            var stock = await _context.ItensStock.AsNoTracking().ToListAsync();
            var acompanhamentos = await _context.Acompanhamentos.AsNoTracking().ToListAsync();
            var postos = await _context.PostosPreparacao.AsNoTracking().ToListAsync();
            var tiposPedidos = await _context.TiposPedidos.AsNoTracking().ToListAsync();
            var unidades = await _context.UnidadesMedida.AsNoTracking().ToListAsync();
            var metodosPag = await _context.MetodosPagamento.AsNoTracking().ToListAsync();
            var taxasIva = await _context.TaxasIva.AsNoTracking().ToListAsync();

            return Ok(new
            {
                estabelecimentos = estabelecimentos.Select(e => new
                {
                    id = e.Id.ToString(),
                    nome = e.Nome,
                    moeda = e.Moeda,
                    cor_primaria = e.CorPrimaria,
                    cor_secundaria = e.CorSecundaria,
                    logotipo_url = e.LogotipoUrl,
                    telefone = e.Telefone,
                    email = e.Email,
                    endereco = e.Endereco,
                    cidade = e.Cidade,
                    horario_abertura = e.HorarioAbertura,
                    dias_funcionamento = e.DiasFuncionamento,
                    whatsapp = e.Whatsapp,
                    facebook = e.Facebook,
                    instagram = e.Instagram,
                    website = e.Website,
                    descricao_rodape = e.DescricaoRodape,
                    tem_piscina = e.TemPiscina,
                    ativo = e.Ativo
                }),
                utilizadores = utilizadores.Select(u => new
                {
                    id = u.Id.ToString(),
                    codigo = "USR-" + u.Id.ToString()[..4],
                    nome = u.Nome,
                    username = u.Email.Split('@')[0],
                    email = u.Email,
                    perfil = u.Perfil,
                    estabelecimento_id = u.EstabelecimentoId.ToString(),
                    ativo = u.Ativo
                }),
                fornecedores = fornecedores.Select(f => new
                {
                    id = f.Id.ToString(),
                    nome = f.Nome,
                    nuit = f.Nif,
                    nif = f.Nif,
                    contacto = f.ContactoResponsavel,
                    telefone = f.Telefone,
                    email = f.Email,
                    morada = f.Morada
                }),
                categorias = categorias.Select(c => new
                {
                    id = c.Id.ToString(),
                    codigo = c.Codigo,
                    nome = c.Nome,
                    emoji = c.Emoji,
                    ordem = c.Ordem
                }),
                produtos = produtos.Select(p => new
                {
                    id = p.Id.ToString(),
                    codigo_produto = p.CodigoProduto,
                    nome = p.Nome,
                    preco = p.Preco,
                    preco_promocional = p.PrecoPromocional,
                    tem_promocao = p.TemPromocao,
                    categoria_id = p.CategoriaId.ToString(),
                    imagem_url = p.ImagemUrl,
                    estado = p.Estado,
                    em_destaque = p.EmDestaque,
                    visivel_portal = true
                }),
                clientes = clientes.Select(c => new
                {
                    id = c.Id.ToString(),
                    codigo = "CLI-" + c.Id.ToString()[..4],
                    nome = c.Nome,
                    telefone = c.Telefone,
                    email = c.Email,
                    nuit = c.Nif,
                    nif = c.Nif,
                    endereco = c.Morada,
                    morada = c.Morada
                }),
                salas = salas.Select(s => new
                {
                    id = s.Id.ToString(),
                    codigo = "SALA-" + s.Id.ToString()[..4],
                    nome = s.Nome,
                    descricao = s.Descricao,
                    capacidade_mesas = s.CapacidadeMesas
                }),
                mesas = mesas.Select(m => new
                {
                    id = m.Id.ToString(),
                    codigo = "MS-" + m.Id.ToString()[..4],
                    numero_mesa = m.NumeroMesa,
                    capacidade_lugares = m.CapacidadeLugares,
                    estado = m.Estado,
                    qrcode_token = m.QrCodeToken,
                    sala_id = m.SalaId.ToString()
                }),
                stock = stock.Select(s => new
                {
                    id = s.Id.ToString(),
                    nome = s.Nome,
                    unidade = s.UnidadeMedida,
                    qtd = s.QuantidadeAtual,
                    minimo = s.QuantidadeMinima,
                    custo = s.CustoUnitario
                }),
                acompanhamentos = acompanhamentos.Select(a => new
                {
                    id = a.Id.ToString(),
                    nome = a.Nome,
                    tipo = a.TipoSelecao,
                    preco_adicional = a.PrecoAdicional,
                    padrao = (a.TipoSelecao == "obrigatorio"),
                    disponivel = a.Disponivel
                }),
                postos_preparacao = postos.Select(p => new
                {
                    id = p.Id.ToString(),
                    codigo = p.Codigo,
                    descricao = p.Descricao
                }),
                tipos_pedidos = tiposPedidos.Select(t => new
                {
                    id = t.Id.ToString(),
                    codigo = t.Codigo,
                    descricao = t.Descricao
                }),
                unidades_medida = unidades.Select(u => new
                {
                    id = u.Id.ToString(),
                    codigo = u.Codigo,
                    nome = u.Nome,
                    permite_fracionamento = u.PermiteDecimal
                }),
                metodos_pagamento = metodosPag.Select(m => new
                {
                    id = m.Id.ToString(),
                    codigo = m.Codigo,
                    nome = m.Nome,
                    icone = m.Icone,
                    permite_troco = m.PermiteTroco,
                    ativo = m.Ativo
                }),
                taxas_iva = taxasIva.Select(t => new
                {
                    id = t.Id.ToString(),
                    codigo = t.Codigo,
                    iva = t.Iva,
                    moeda = t.Moeda,
                    descricao = t.Descricao,
                    ativo = t.Ativo,
                    padrao = t.Padrao
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estado global da base de dados");
            return StatusCode(500, new { erro = ex.Message });
        }
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SincronizarCadastrosFrontend([FromBody] JsonElement payload)
    {
        try
        {
            var estId = _tenantService.EstabelecimentoId ?? ToGuid("1", "est");
            var estExiste = await _context.Estabelecimentos.AnyAsync(e => e.Id == estId);
            if (!estExiste)
            {
                var novoEst = new Estabelecimento
                {
                    Id = estId,
                    Nome = "Restaurante Sabor & Brasa",
                    Slug = "sabor-brasa",
                    Moeda = "MZN",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Estabelecimentos.Add(novoEst);
                await _context.SaveChangesAsync();
            }

            // Estabelecimentos
            if (payload.TryGetProperty("estabelecimentos", out var estEl) && estEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in estEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "est");
                    var dbEst = await _context.Estabelecimentos.FirstOrDefaultAsync(e => e.Id == guid);
                    if (dbEst == null)
                    {
                        dbEst = new Estabelecimento { Id = guid, CreatedAt = DateTime.UtcNow };
                        _context.Estabelecimentos.Add(dbEst);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbEst.Nome = nome.GetString() ?? dbEst.Nome;
                    if (item.TryGetProperty("moeda", out var moeda)) dbEst.Moeda = moeda.GetString() ?? dbEst.Moeda;
                    if (item.TryGetProperty("cor_primaria", out var corP)) dbEst.CorPrimaria = corP.GetString() ?? dbEst.CorPrimaria;
                    if (item.TryGetProperty("cor_secundaria", out var corS)) dbEst.CorSecundaria = corS.GetString() ?? dbEst.CorSecundaria;
                    if (item.TryGetProperty("logotipo_url", out var logo)) dbEst.LogotipoUrl = logo.GetString() ?? dbEst.LogotipoUrl;
                    if (item.TryGetProperty("telefone", out var tel)) dbEst.Telefone = tel.GetString() ?? dbEst.Telefone;
                    if (item.TryGetProperty("email", out var email)) dbEst.Email = email.GetString() ?? dbEst.Email;
                    if (item.TryGetProperty("endereco", out var end)) dbEst.Endereco = end.GetString() ?? dbEst.Endereco;
                    if (item.TryGetProperty("cidade", out var cid)) dbEst.Cidade = cid.GetString() ?? dbEst.Cidade;
                    if (item.TryGetProperty("horario_abertura", out var hor)) dbEst.HorarioAbertura = hor.GetString() ?? dbEst.HorarioAbertura;
                    if (item.TryGetProperty("dias_funcionamento", out var dias)) dbEst.DiasFuncionamento = dias.GetString() ?? dbEst.DiasFuncionamento;
                    if (item.TryGetProperty("whatsapp", out var wpp)) dbEst.Whatsapp = wpp.GetString() ?? dbEst.Whatsapp;
                    if (item.TryGetProperty("facebook", out var fb)) dbEst.Facebook = fb.GetString() ?? dbEst.Facebook;
                    if (item.TryGetProperty("instagram", out var ig)) dbEst.Instagram = ig.GetString() ?? dbEst.Instagram;
                    if (item.TryGetProperty("website", out var site)) dbEst.Website = site.GetString() ?? dbEst.Website;
                    if (item.TryGetProperty("descricao_rodape", out var rod)) dbEst.DescricaoRodape = rod.GetString() ?? dbEst.DescricaoRodape;
                    if (item.TryGetProperty("tem_piscina", out var pisc)) dbEst.TemPiscina = pisc.GetBoolean();
                    if (item.TryGetProperty("ativo", out var ativ)) dbEst.Ativo = ativ.GetBoolean();
                }
            }

            // Fornecedores
            if (payload.TryGetProperty("fornecedores", out var fornEl) && fornEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in fornEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "forn");
                    var dbForn = await _context.Fornecedores.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == guid);
                    if (dbForn == null)
                    {
                        dbForn = new Fornecedor { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.Fornecedores.Add(dbForn);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbForn.Nome = nome.GetString() ?? dbForn.Nome;
                    if (item.TryGetProperty("nuit", out var nuit)) dbForn.Nif = nuit.GetString() ?? dbForn.Nif;
                    else if (item.TryGetProperty("nif", out var nif)) dbForn.Nif = nif.GetString() ?? dbForn.Nif;
                    if (item.TryGetProperty("contacto", out var cont)) dbForn.ContactoResponsavel = cont.GetString();
                    if (item.TryGetProperty("telefone", out var tel)) dbForn.Telefone = tel.GetString();
                    if (item.TryGetProperty("email", out var email)) dbForn.Email = email.GetString();
                    if (item.TryGetProperty("morada", out var morada)) dbForn.Morada = morada.GetString();
                }
            }

            // Clientes
            if (payload.TryGetProperty("clientes", out var cliEl) && cliEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in cliEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "cli");
                    var dbCli = await _context.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == guid);
                    if (dbCli == null)
                    {
                        dbCli = new Cliente { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.Clientes.Add(dbCli);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbCli.Nome = nome.GetString() ?? dbCli.Nome;
                    if (item.TryGetProperty("telefone", out var tel)) dbCli.Telefone = tel.GetString() ?? dbCli.Telefone;
                    if (item.TryGetProperty("email", out var email)) dbCli.Email = email.GetString();
                    if (item.TryGetProperty("nuit", out var nuit)) dbCli.Nif = nuit.GetString();
                    else if (item.TryGetProperty("nif", out var nif)) dbCli.Nif = nif.GetString();
                    if (item.TryGetProperty("endereco", out var end)) dbCli.Morada = end.GetString();
                    else if (item.TryGetProperty("morada", out var morada)) dbCli.Morada = morada.GetString();
                }
            }

            // Categorias
            if (payload.TryGetProperty("categorias", out var catEl) && catEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in catEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "cat");
                    var dbCat = await _context.Categorias.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == guid);
                    if (dbCat == null)
                    {
                        dbCat = new Categoria { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.Categorias.Add(dbCat);
                    }
                    if (item.TryGetProperty("codigo", out var cod)) dbCat.Codigo = cod.GetString() ?? "CAT-01";
                    if (item.TryGetProperty("nome", out var nome)) dbCat.Nome = nome.GetString() ?? dbCat.Nome;
                    if (item.TryGetProperty("emoji", out var emoji)) dbCat.Emoji = emoji.GetString() ?? "🍽️";
                    if (item.TryGetProperty("ordem", out var ord)) dbCat.Ordem = ord.GetInt32();
                }
                await _context.SaveChangesAsync();
            }

            // Produtos
            if (payload.TryGetProperty("produtos", out var prodEl) && prodEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prodEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "prod");
                    var dbProd = await _context.Produtos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == guid);
                    if (dbProd == null)
                    {
                        dbProd = new Produto { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.Produtos.Add(dbProd);
                    }
                    if (item.TryGetProperty("codigo_produto", out var cod)) dbProd.CodigoProduto = cod.GetString() ?? "PRD-001";
                    if (item.TryGetProperty("nome", out var nome)) dbProd.Nome = nome.GetString() ?? dbProd.Nome;
                    if (item.TryGetProperty("preco", out var preco)) dbProd.Preco = (decimal)preco.GetDouble();
                    if (item.TryGetProperty("preco_promocional", out var precoP) && precoP.ValueKind == JsonValueKind.Number)
                        dbProd.PrecoPromocional = (decimal)precoP.GetDouble();
                    if (item.TryGetProperty("tem_promocao", out var temP)) dbProd.TemPromocao = temP.GetBoolean();
                    if (item.TryGetProperty("imagem_url", out var img)) dbProd.ImagemUrl = img.GetString();
                    if (item.TryGetProperty("em_destaque", out var dest)) dbProd.EmDestaque = dest.GetBoolean();
                    if (item.TryGetProperty("estado", out var est)) dbProd.Estado = est.GetString() ?? "disponivel";

                    if (item.TryGetProperty("categoria_id", out var cId))
                    {
                        var cGuid = ToGuid(cId.GetString(), "cat");
                        var catExists = await _context.Categorias.AnyAsync(c => c.Id == cGuid);
                        if (catExists) dbProd.CategoriaId = cGuid;
                    }
                }
            }

            // Salas
            if (payload.TryGetProperty("salas", out var salasEl) && salasEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in salasEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "sala");
                    var dbSala = await _context.Salas.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == guid);
                    if (dbSala == null)
                    {
                        dbSala = new Sala { Id = guid, EstabelecimentoId = estId };
                        _context.Salas.Add(dbSala);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbSala.Nome = nome.GetString() ?? dbSala.Nome;
                    if (item.TryGetProperty("descricao", out var desc)) dbSala.Descricao = desc.GetString();
                    if (item.TryGetProperty("capacidade_mesas", out var cap)) dbSala.CapacidadeMesas = cap.GetInt32();
                }
                await _context.SaveChangesAsync();
            }

            // Mesas
            if (payload.TryGetProperty("mesas", out var mesasEl) && mesasEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in mesasEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "mesa");
                    var dbMesa = await _context.Mesas.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == guid);
                    if (dbMesa == null)
                    {
                        dbMesa = new Mesa { Id = guid, EstabelecimentoId = estId };
                        _context.Mesas.Add(dbMesa);
                    }
                    if (item.TryGetProperty("numero_mesa", out var num)) dbMesa.NumeroMesa = num.GetString() ?? dbMesa.NumeroMesa;
                    if (item.TryGetProperty("capacidade_lugares", out var cap)) dbMesa.CapacidadeLugares = cap.GetInt32();
                    if (item.TryGetProperty("estado", out var estado)) dbMesa.Estado = estado.GetString() ?? "LIVRE";
                    if (item.TryGetProperty("qrcode_token", out var qr)) dbMesa.QrCodeToken = qr.GetString();

                    if (item.TryGetProperty("sala_id", out var sId))
                    {
                        var sGuid = ToGuid(sId.GetString(), "sala");
                        if (await _context.Salas.AnyAsync(s => s.Id == sGuid))
                            dbMesa.SalaId = sGuid;
                    }
                }
            }

            // Stock
            if (payload.TryGetProperty("stock", out var stockEl) && stockEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in stockEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "stock");
                    var dbItem = await _context.ItensStock.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == guid);
                    if (dbItem == null)
                    {
                        dbItem = new ItemStock { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.ItensStock.Add(dbItem);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbItem.Nome = nome.GetString() ?? dbItem.Nome;
                    if (item.TryGetProperty("unidade", out var unid)) dbItem.UnidadeMedida = unid.GetString() ?? "UN";
                    if (item.TryGetProperty("qtd", out var qtd)) dbItem.QuantidadeAtual = (decimal)qtd.GetDouble();
                    if (item.TryGetProperty("minimo", out var min)) dbItem.QuantidadeMinima = (decimal)min.GetDouble();
                    if (item.TryGetProperty("custo", out var custo)) dbItem.CustoUnitario = (decimal)custo.GetDouble();
                }
            }

            // Acompanhamentos
            if (payload.TryGetProperty("acompanhamentos", out var acompEl) && acompEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in acompEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "acomp");
                    var dbAcomp = await _context.Acompanhamentos.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == guid);
                    if (dbAcomp == null)
                    {
                        dbAcomp = new Acompanhamento { Id = guid, EstabelecimentoId = estId };
                        _context.Acompanhamentos.Add(dbAcomp);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbAcomp.Nome = nome.GetString() ?? dbAcomp.Nome;
                    if (item.TryGetProperty("tipo", out var tipo)) dbAcomp.TipoSelecao = tipo.GetString() ?? "opcional_pago";
                    if (item.TryGetProperty("preco_adicional", out var preco)) dbAcomp.PrecoAdicional = (decimal)preco.GetDouble();
                    if (item.TryGetProperty("disponivel", out var disp)) dbAcomp.Disponivel = disp.GetBoolean();
                }
            }

            // Postos de Preparação
            if (payload.TryGetProperty("postos_preparacao", out var postosEl) && postosEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in postosEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "posto");
                    var dbPosto = await _context.PostosPreparacao.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == guid);
                    if (dbPosto == null)
                    {
                        dbPosto = new PostoPreparacao { Id = guid, EstabelecimentoId = estId };
                        _context.PostosPreparacao.Add(dbPosto);
                    }
                    if (item.TryGetProperty("codigo", out var cod)) dbPosto.Codigo = cod.GetString() ?? "PST-01";
                    if (item.TryGetProperty("descricao", out var desc)) dbPosto.Descricao = desc.GetString() ?? dbPosto.Descricao;
                }
            }

            // Tipos de Pedidos
            if (payload.TryGetProperty("tipos_pedidos", out var tiposEl) && tiposEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in tiposEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "tp");
                    var dbTipo = await _context.TiposPedidos.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == guid);
                    if (dbTipo == null)
                    {
                        dbTipo = new TipoPedido { Id = guid, EstabelecimentoId = estId };
                        _context.TiposPedidos.Add(dbTipo);
                    }
                    if (item.TryGetProperty("codigo", out var cod)) dbTipo.Codigo = cod.GetString() ?? "TP-01";
                    if (item.TryGetProperty("descricao", out var desc)) dbTipo.Descricao = desc.GetString() ?? dbTipo.Descricao;
                }
            }

            // Métodos de Pagamento
            if (payload.TryGetProperty("metodos_pagamento", out var metEl) && metEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in metEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "pag");
                    var dbMet = await _context.MetodosPagamento.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == guid);
                    if (dbMet == null)
                    {
                        dbMet = new MetodoPagamento { Id = guid, EstabelecimentoId = estId };
                        _context.MetodosPagamento.Add(dbMet);
                    }
                    if (item.TryGetProperty("codigo", out var cod)) dbMet.Codigo = cod.GetString() ?? "PAG-01";
                    if (item.TryGetProperty("nome", out var nome)) dbMet.Nome = nome.GetString() ?? dbMet.Nome;
                    if (item.TryGetProperty("icone", out var icone)) dbMet.Icone = icone.GetString() ?? "fa-solid fa-money-bill";
                    if (item.TryGetProperty("permite_troco", out var troco)) dbMet.PermiteTroco = troco.GetBoolean();
                    if (item.TryGetProperty("ativo", out var ativ)) dbMet.Ativo = ativ.GetBoolean();
                }
            }

            // Taxas de IVA
            if (payload.TryGetProperty("taxas_iva", out var ivaEl) && ivaEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in ivaEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "iva");
                    var dbIva = await _context.TaxasIva.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == guid);
                    if (dbIva == null)
                    {
                        dbIva = new TaxaIva { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.TaxasIva.Add(dbIva);
                    }
                    if (item.TryGetProperty("codigo", out var cod)) dbIva.Codigo = cod.GetString() ?? "IVA-16";
                    if (item.TryGetProperty("iva", out var ivaVal)) dbIva.Iva = (decimal)ivaVal.GetDouble();
                    if (item.TryGetProperty("moeda", out var moeda)) dbIva.Moeda = moeda.GetString() ?? "MZN";
                    if (item.TryGetProperty("descricao", out var desc)) dbIva.Descricao = desc.GetString();
                    if (item.TryGetProperty("ativo", out var ativ)) dbIva.Ativo = ativ.GetBoolean();
                    if (item.TryGetProperty("padrao", out var padrao)) dbIva.Padrao = padrao.GetBoolean();
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { status = "success", message = "Cadastros sincronizados com a base de dados com sucesso." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar dados do Frontend para o PostgreSQL/Supabase");
            return StatusCode(500, new { status = "error", erro = ex.Message });
        }
    }
}
