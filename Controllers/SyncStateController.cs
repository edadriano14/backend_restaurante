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
            if (!utilizadores.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var uAdmin = new Utilizador
                {
                    Id = ToGuid("1", "usr"),
                    EstabelecimentoId = estId,
                    Nome = "Administrador Geral",
                    Email = "admin@restaurante.com",
                    Perfil = "ADMIN_GERAL",
                    SenhaHash = "AQAAAAEAACcQAAAAEJ...", // placeholder
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                };
                var uGestor = new Utilizador
                {
                    Id = ToGuid("2", "usr"),
                    EstabelecimentoId = estId,
                    Nome = "Gestor do Restaurante",
                    Email = "gestor@saborbrasa.com",
                    Perfil = "GESTOR",
                    SenhaHash = "AQAAAAEAACcQAAAAEJ...",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                };
                var uCaixa = new Utilizador
                {
                    Id = ToGuid("3", "usr"),
                    EstabelecimentoId = estId,
                    Nome = "Operador de Caixa 01",
                    Email = "caixa1@saborbrasa.com",
                    Perfil = "OPERADOR",
                    SenhaHash = "AQAAAAEAACcQAAAAEJ...",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                };
                var uStock = new Utilizador
                {
                    Id = ToGuid("4", "usr"),
                    EstabelecimentoId = estId,
                    Nome = "Fiel de Armazém / Stock",
                    Email = "stock@saborbrasa.com",
                    Perfil = "STOCK",
                    SenhaHash = "AQAAAAEAACcQAAAAEJ...",
                    Ativo = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Utilizadores.AddRange(uAdmin, uGestor, uCaixa, uStock);
                await _context.SaveChangesAsync();
                utilizadores = await _context.Utilizadores.AsNoTracking().ToListAsync();
            }
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
            if (!unidades.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var umKg = new UnidadeMedida { Id = ToGuid("1", "um"), EstabelecimentoId = estId, Codigo = "KG", Nome = "Quilograma", PermiteDecimal = true };
                var umLt = new UnidadeMedida { Id = ToGuid("2", "um"), EstabelecimentoId = estId, Codigo = "LT", Nome = "Litro", PermiteDecimal = true };
                var umUn = new UnidadeMedida { Id = ToGuid("3", "um"), EstabelecimentoId = estId, Codigo = "UN", Nome = "Unidade", PermiteDecimal = false };
                var umCx = new UnidadeMedida { Id = ToGuid("4", "um"), EstabelecimentoId = estId, Codigo = "CX", Nome = "Caixa", PermiteDecimal = false };
                var umPct = new UnidadeMedida { Id = ToGuid("5", "um"), EstabelecimentoId = estId, Codigo = "PCT", Nome = "Pacote", PermiteDecimal = false };
                var umDose = new UnidadeMedida { Id = ToGuid("6", "um"), EstabelecimentoId = estId, Codigo = "DOSE", Nome = "Dose / Porção", PermiteDecimal = false };
                _context.UnidadesMedida.AddRange(umKg, umLt, umUn, umCx, umPct, umDose);
                await _context.SaveChangesAsync();
                unidades = await _context.UnidadesMedida.AsNoTracking().ToListAsync();
            }

            var metodosPag = await _context.MetodosPagamento.AsNoTracking().ToListAsync();
            var taxasIva = await _context.TaxasIva.AsNoTracking().ToListAsync();

            var armazens = await _context.Armazens.AsNoTracking().ToListAsync();
            if (!armazens.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var armP = new Armazem { Id = ToGuid("1", "arm"), EstabelecimentoId = estId, Codigo = "ARM-01", Nome = "Armazém Geral & Secos", Tipo = "CENTRAL", Ativo = true };
                var armB = new Armazem { Id = ToGuid("2", "arm"), EstabelecimentoId = estId, Codigo = "ARM-02", Nome = "Depósito de Bebidas & Frio", Tipo = "REFRIGERADO", Ativo = true };
                var armC = new Armazem { Id = ToGuid("3", "arm"), EstabelecimentoId = estId, Codigo = "ARM-03", Nome = "Despensa da Cozinha", Tipo = "COZINHA", Ativo = true };
                _context.Armazens.AddRange(armP, armB, armC);
                await _context.SaveChangesAsync();
                armazens = await _context.Armazens.AsNoTracking().ToListAsync();
            }

            var locais = await _context.LocaisArmazenamento.AsNoTracking().ToListAsync();
            if (!locais.Any())
            {
                var armId1 = armazens.FirstOrDefault()?.Id ?? ToGuid("1", "arm");
                var armId2 = armazens.Skip(1).FirstOrDefault()?.Id ?? ToGuid("2", "arm");
                var loc1 = new LocalArmazenamento { Id = ToGuid("1", "loc"), ArmazemId = armId1, Codigo = "EST-A1", Descricao = "Estante A - Prateleira 1 (Cereais & Farinhas)", TipoEstrutura = "ESTANTE", Ativo = true };
                var loc2 = new LocalArmazenamento { Id = ToGuid("2", "loc"), ArmazemId = armId1, Codigo = "EST-A2", Descricao = "Estante A - Prateleira 2 (Conservas & Óleos)", TipoEstrutura = "ESTANTE", Ativo = true };
                var loc3 = new LocalArmazenamento { Id = ToGuid("3", "loc"), ArmazemId = armId2, Codigo = "CAM-01", Descricao = "Câmara Frigorífica de Carnes & Aves", TipoEstrutura = "CONGELADOR", Ativo = true };
                var loc4 = new LocalArmazenamento { Id = ToGuid("4", "loc"), ArmazemId = armId2, Codigo = "GEL-01", Descricao = "Geleira Industrial de Bebidas & Sumos", TipoEstrutura = "GELEIRA", Ativo = true };
                _context.LocaisArmazenamento.AddRange(loc1, loc2, loc3, loc4);
                await _context.SaveChangesAsync();
                locais = await _context.LocaisArmazenamento.AsNoTracking().ToListAsync();
            }

            var canais = await _context.CanaisPagamento.AsNoTracking().ToListAsync();
            if (!canais.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var cp1 = new CanalPagamento { Id = ToGuid("1", "cp"), EstabelecimentoId = estId, Codigo = "CP-MPESA-01", CodigoOperadora = "MPESA", Descricao = "M-Pesa Restaurante Principal", NumeroAssociado = "840001122", TitularConta = "Sabor & Brasa Lda", InstrucoesPagamento = "Pagar via M-Pesa Balcão", Ativo = true, CreatedAt = DateTime.UtcNow };
                var cp2 = new CanalPagamento { Id = ToGuid("2", "cp"), EstabelecimentoId = estId, Codigo = "CP-EMOLA-01", CodigoOperadora = "EMOLA", Descricao = "e-Mola Restaurante Principal", NumeroAssociado = "860001122", TitularConta = "Sabor & Brasa Lda", InstrucoesPagamento = "Pagar via e-Mola Moçambique", Ativo = true, CreatedAt = DateTime.UtcNow };
                var cp3 = new CanalPagamento { Id = ToGuid("3", "cp"), EstabelecimentoId = estId, Codigo = "CP-BIM-POS", CodigoOperadora = "POS", Descricao = "POS Millennium BIM (Cartões)", NumeroAssociado = "POS-BIM-01", TitularConta = "Sabor & Brasa Lda", InstrucoesPagamento = "Cartão de Débito/Crédito", Ativo = true, CreatedAt = DateTime.UtcNow };
                _context.CanaisPagamento.AddRange(cp1, cp2, cp3);
                await _context.SaveChangesAsync();
                canais = await _context.CanaisPagamento.AsNoTracking().ToListAsync();
            }

            var categoriasStock = await _context.CategoriasStock.AsNoTracking().ToListAsync();
            if (!categoriasStock.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var cs1 = new CategoriaStock { Id = ToGuid("1", "csk"), EstabelecimentoId = estId, Codigo = "CSK-01", Nome = "Carnes, Aves & Peixes", Descricao = "Proteínas e insumos de corte" };
                var cs2 = new CategoriaStock { Id = ToGuid("2", "csk"), EstabelecimentoId = estId, Codigo = "CSK-02", Nome = "Bebidas & Refrigerantes", Descricao = "Cervejas, refrigerantes, água e licores" };
                var cs3 = new CategoriaStock { Id = ToGuid("3", "csk"), EstabelecimentoId = estId, Codigo = "CSK-03", Nome = "Secos & Mercearia", Descricao = "Arroz, farinhas, óleos e temperos" };
                var cs4 = new CategoriaStock { Id = ToGuid("4", "csk"), EstabelecimentoId = estId, Codigo = "CSK-04", Nome = "Hortícolas & Legumes", Descricao = "Produtos frescos da horta" };
                _context.CategoriasStock.AddRange(cs1, cs2, cs3, cs4);
                await _context.SaveChangesAsync();
                categoriasStock = await _context.CategoriasStock.AsNoTracking().ToListAsync();
            }

            var caixasSessoes = await _context.CaixasSessoes.AsNoTracking().OrderByDescending(c => c.AbertoEm).Take(50).ToListAsync();
            if (!caixasSessoes.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var uId = utilizadores.FirstOrDefault()?.Id ?? ToGuid("1", "usr");
                var csPadrao = new CaixaSessao
                {
                    Id = ToGuid("1", "cs"),
                    EstabelecimentoId = estId,
                    UtilizadorId = uId,
                    Codigo = "CX-001",
                    SaldoAbertura = 1500.00m,
                    SaldoFecho = null,
                    Estado = "ABERTO",
                    AbertoEm = DateTime.UtcNow.Date.AddHours(8),
                    Observacoes = "Abertura automática de turno"
                };
                _context.CaixasSessoes.Add(csPadrao);
                await _context.SaveChangesAsync();
                caixasSessoes = await _context.CaixasSessoes.AsNoTracking().ToListAsync();
            }
            var compras = await _context.Compras.AsNoTracking().OrderByDescending(c => c.DataCompra).Take(100).ToListAsync();
            var itensCompra = await _context.ItensCompra.AsNoTracking().Take(500).ToListAsync();
            if (!compras.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var fornId = fornecedores.FirstOrDefault()?.Id;
                var itemStk = stock.FirstOrDefault();
                var cmpId = ToGuid("1", "cmp");

                var cmpPadrao = new Compra
                {
                    Id = cmpId,
                    EstabelecimentoId = estId,
                    FornecedorId = fornId,
                    Codigo = "CMP-001",
                    NumeroFatura = "FT-2026/089",
                    DataCompra = DateTime.UtcNow.AddDays(-2),
                    ValorTotal = 33750.00m,
                    Status = "CONCLUIDA",
                    Observacoes = "Compra inicial de carnes e insumos para abertura"
                };
                _context.Compras.Add(cmpPadrao);

                if (itemStk != null)
                {
                    var itCmp = new ItemCompra
                    {
                        Id = ToGuid("1", "itcmp"),
                        Codigo = "IC-001",
                        CompraId = cmpId,
                        ItemStockId = itemStk.Id,
                        Quantidade = 45m,
                        PrecoUnitario = itemStk.CustoUnitario > 0 ? itemStk.CustoUnitario : 750.00m
                    };
                    _context.ItensCompra.Add(itCmp);
                }

                await _context.SaveChangesAsync();
                compras = await _context.Compras.AsNoTracking().ToListAsync();
                itensCompra = await _context.ItensCompra.AsNoTracking().ToListAsync();
            }

            var guiasEntrada = await _context.GuiasEntrada.AsNoTracking().OrderByDescending(g => g.DataEmissao).Take(100).ToListAsync();
            var itensGuia = await _context.ItensGuiaEntrada.AsNoTracking().Take(500).ToListAsync();
            if (!guiasEntrada.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var fornId = fornecedores.FirstOrDefault()?.Id;
                var itemStk = stock.FirstOrDefault();
                var geId = ToGuid("1", "ge");

                var gePadrao = new GuiaEntrada
                {
                    Id = geId,
                    EstabelecimentoId = estId,
                    FornecedorId = fornId,
                    Codigo = "GE-001",
                    NumeroGuia = "GE-2026-001",
                    DataEmissao = DateTime.UtcNow.AddDays(-1),
                    ValorTotal = 33750.00m,
                    Status = "PROCESSADA",
                    Observacoes = "Recepção de mercadorias no armazém central"
                };
                _context.GuiasEntrada.Add(gePadrao);

                if (itemStk != null)
                {
                    var itGe = new ItemGuiaEntrada
                    {
                        Id = ToGuid("1", "itge"),
                        Codigo = "IGE-001",
                        GuiaEntradaId = geId,
                        ItemStockId = itemStk.Id,
                        Quantidade = 45m,
                        PrecoCusto = itemStk.CustoUnitario > 0 ? itemStk.CustoUnitario : 750.00m
                    };
                    _context.ItensGuiaEntrada.Add(itGe);
                }

                await _context.SaveChangesAsync();
                guiasEntrada = await _context.GuiasEntrada.AsNoTracking().ToListAsync();
                itensGuia = await _context.ItensGuiaEntrada.AsNoTracking().ToListAsync();
            }

            var pedidosVendas = await _context.PedidosVendas.AsNoTracking().OrderByDescending(p => p.CreatedAt).Take(100).ToListAsync();
            var itensPedidos = await _context.ItensPedidoVenda.AsNoTracking().Take(500).ToListAsync();
            if (!pedidosVendas.Any())
            {
                var estId = estabelecimentos.FirstOrDefault()?.Id ?? ToGuid("1", "est");
                var prod = produtos.FirstOrDefault();
                var pvId = ToGuid("1", "pv");

                var pvPadrao = new PedidoVenda
                {
                    Id = pvId,
                    EstabelecimentoId = estId,
                    Codigo = "PV-001",
                    NumeroPedido = "PED-001",
                    NumeroMesa = "Mesa 01",
                    ClienteNome = "Cliente Balcão",
                    TipoOrigem = "PDV_BALCAO",
                    MetodoPagamento = "NUMERARIO",
                    Subtotal = prod != null ? prod.Preco : 1250.00m,
                    ValorTotal = prod != null ? prod.Preco : 1250.00m,
                    Estado = "ENTREGUE",
                    Observacoes = "Venda inicial balcão",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                };
                _context.PedidosVendas.Add(pvPadrao);

                if (prod != null)
                {
                    var itPv = new ItemPedidoVenda
                    {
                        Id = ToGuid("1", "itpv"),
                        Codigo = "IPV-001",
                        PedidoId = pvId,
                        ProdutoId = prod.Id,
                        Quantidade = 1m,
                        PrecoUnitario = prod.Preco,
                        Subtotal = prod.Preco
                    };
                    _context.ItensPedidoVenda.Add(itPv);
                }

                await _context.SaveChangesAsync();
                pedidosVendas = await _context.PedidosVendas.AsNoTracking().ToListAsync();
                itensPedidos = await _context.ItensPedidoVenda.AsNoTracking().ToListAsync();
            }
            var pedidosOnline = await _context.PedidosOnline.AsNoTracking().OrderByDescending(p => p.DataPedido).Take(100).ToListAsync();

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
                    iva = t.Iva ?? 16.00m,
                    moeda = t.Moeda,
                    descricao = t.Descricao,
                    ativo = t.Ativo,
                    padrao = t.Padrao
                }),
                armazens = armazens.Select(a => new
                {
                    id = a.Id.ToString(),
                    codigo = a.Codigo,
                    nome = a.Nome,
                    tipo = a.Tipo,
                    ativo = a.Ativo
                }),
                locais_armazenamento = locais.Select(l => new
                {
                    id = l.Id.ToString(),
                    armazem_id = l.ArmazemId.ToString(),
                    tipo_estrutura = l.TipoEstrutura,
                    codigo = l.Codigo,
                    descricao = l.Descricao,
                    ativo = l.Ativo
                }),
                canais_pagamento = canais.Select(cp => new
                {
                    id = cp.Id.ToString(),
                    codigo = cp.Codigo,
                    codigo_operadora = cp.CodigoOperadora,
                    descricao = cp.Descricao,
                    numero_associado = cp.NumeroAssociado,
                    titular_conta = cp.TitularConta,
                    instrucoes_pagamento = cp.InstrucoesPagamento,
                    ativo = cp.Ativo
                }),
                categorias_stock = categoriasStock.Select(cs => new
                {
                    id = cs.Id.ToString(),
                    codigo = cs.Codigo,
                    nome = cs.Nome,
                    descricao = cs.Descricao
                }),
                caixas_sessoes = caixasSessoes.Select(cs => new
                {
                    id = cs.Id.ToString(),
                    codigo = cs.Codigo,
                    utilizador_id = cs.UtilizadorId.ToString(),
                    saldo_abertura = cs.SaldoAbertura,
                    saldo_fecho = cs.SaldoFecho,
                    estado = cs.Estado,
                    aberto_em = cs.AbertoEm.ToString("s"),
                    fechado_em = cs.FechadoEm.HasValue ? cs.FechadoEm.Value.ToString("s") : null,
                    observacoes = cs.Observacoes
                }),
                compras = compras.Select(cmp => new
                {
                    id = cmp.Id.ToString(),
                    codigo = cmp.Codigo,
                    fornecedor_id = cmp.FornecedorId.HasValue ? cmp.FornecedorId.Value.ToString() : null,
                    numero_fatura = cmp.NumeroFatura,
                    data_compra = cmp.DataCompra.ToString("s"),
                    valor_total = cmp.ValorTotal,
                    status = cmp.Status,
                    observacoes = cmp.Observacoes
                }),
                itens_compra = itensCompra.Select(ic => new
                {
                    id = ic.Id.ToString(),
                    codigo = ic.Codigo,
                    compra_id = ic.CompraId.ToString(),
                    item_stock_id = ic.ItemStockId.ToString(),
                    quantidade = ic.Quantidade,
                    preco_unitario = ic.PrecoUnitario,
                    subtotal = ic.Subtotal
                }),
                guias_entrada = guiasEntrada.Select(ge => new
                {
                    id = ge.Id.ToString(),
                    codigo = ge.Codigo,
                    fornecedor_id = ge.FornecedorId.HasValue ? ge.FornecedorId.Value.ToString() : null,
                    numero_guia = ge.NumeroGuia,
                    data_emissao = ge.DataEmissao.ToString("s"),
                    valor_total = ge.ValorTotal,
                    status = ge.Status,
                    observacoes = ge.Observacoes
                }),
                itens_guia_entrada = itensGuia.Select(ig => new
                {
                    id = ig.Id.ToString(),
                    codigo = ig.Codigo,
                    guia_entrada_id = ig.GuiaEntradaId.ToString(),
                    item_stock_id = ig.ItemStockId.ToString(),
                    quantidade = ig.Quantidade,
                    preco_custo = ig.PrecoCusto,
                    subtotal = ig.Subtotal
                }),
                pedidos_vendas = pedidosVendas.Select(pv => new
                {
                    id = pv.Id.ToString(),
                    codigo = pv.Codigo,
                    caixa_sessao_id = pv.CaixaSessaoId.HasValue ? pv.CaixaSessaoId.Value.ToString() : null,
                    numero_pedido = pv.NumeroPedido,
                    numero_mesa = pv.NumeroMesa,
                    cliente_nome = pv.ClienteNome,
                    cliente_telefone = pv.ClienteTelefone,
                    tipo_origem = pv.TipoOrigem,
                    metodo_pagamento = pv.MetodoPagamento,
                    subtotal = pv.Subtotal,
                    valor_total = pv.ValorTotal,
                    estado = pv.Estado,
                    observacoes = pv.Observacoes,
                    created_at = pv.CreatedAt.ToString("s")
                }),
                itens_pedidos_vendas = itensPedidos.Select(ipv => new
                {
                    id = ipv.Id.ToString(),
                    codigo = ipv.Codigo,
                    pedido_id = ipv.PedidoId.ToString(),
                    produto_id = ipv.ProdutoId.ToString(),
                    quantidade = ipv.Quantidade,
                    preco_unitario = ipv.PrecoUnitario,
                    opcoes_selecionadas = ipv.OpcoesSelecionadas,
                    subtotal = ipv.Subtotal
                }),
                pedidos_online = pedidosOnline.Select(po => new
                {
                    id = po.Id.ToString(),
                    codigo = po.Codigo,
                    numero_pedido = po.NumeroPedido,
                    cliente_nome = po.ClienteNome,
                    cliente_telefone = po.ClienteTelefone,
                    tipo_entrega = po.TipoEntrega,
                    numero_mesa = po.NumeroMesa,
                    endereco_entrega = po.EnderecoEntrega,
                    operadora_pagamento = po.OperadoraPagamento,
                    referencia_pagamento = po.ReferenciaPagamento,
                    subtotal = po.Subtotal,
                    taxa_entrega = po.TaxaEntrega,
                    total = po.Total,
                    estado = po.Estado,
                    data_pedido = po.DataPedido.ToString("s")
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
                    if (item.TryGetProperty("codigo", out var codForn)) dbForn.Codigo = codForn.GetString() ?? dbForn.Codigo;
                    if (item.TryGetProperty("nuit", out var nuit))
                    {
                        var nStr = nuit.GetString()?.Trim();
                        var d = new string(nStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                        if (d.Length == 10) dbForn.Nif = d;
                        else if (!string.IsNullOrEmpty(nStr)) dbForn.Nif = nStr;
                    }
                    else if (item.TryGetProperty("nif", out var nif))
                    {
                        var nStr = nif.GetString()?.Trim();
                        var d = new string(nStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                        if (d.Length == 10) dbForn.Nif = d;
                        else if (!string.IsNullOrEmpty(nStr)) dbForn.Nif = nStr;
                    }
                    if (item.TryGetProperty("contacto", out var cont)) dbForn.ContactoResponsavel = cont.GetString();
                    if (item.TryGetProperty("telefone", out var tel))
                    {
                        var tStr = tel.GetString()?.Trim();
                        var d = new string(tStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                        if (d.Length == 10) dbForn.Telefone = d;
                        else if (!string.IsNullOrEmpty(tStr)) dbForn.Telefone = tStr;
                    }
                    if (item.TryGetProperty("email", out var email))
                    {
                        var eStr = email.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(eStr) && eStr.Contains('@')) dbForn.Email = eStr;
                    }
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
                    if (item.TryGetProperty("codigo", out var codCli)) dbCli.Codigo = codCli.GetString() ?? dbCli.Codigo;
                    if (item.TryGetProperty("telefone", out var tel))
                    {
                        var tStr = tel.GetString()?.Trim();
                        var d = new string(tStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                        if (d.Length == 10) dbCli.Telefone = d;
                        else if (!string.IsNullOrEmpty(tStr)) dbCli.Telefone = tStr;
                    }
                    if (item.TryGetProperty("email", out var email))
                    {
                        var eStr = email.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(eStr) && eStr.Contains('@')) dbCli.Email = eStr;
                    }
                    if (item.TryGetProperty("nuit", out var nuit))
                    {
                        var nStr = nuit.GetString()?.Trim();
                        var d = new string(nStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                        if (d.Length == 10) dbCli.Nif = d;
                        else if (!string.IsNullOrEmpty(nStr)) dbCli.Nif = nStr;
                    }
                    else if (item.TryGetProperty("nif", out var nif))
                    {
                        var nStr = nif.GetString()?.Trim();
                        var d = new string(nStr?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                        if (d.Length == 10) dbCli.Nif = d;
                        else if (!string.IsNullOrEmpty(nStr)) dbCli.Nif = nStr;
                    }
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

            // Utilizadores
            if (payload.TryGetProperty("utilizadores", out var usersEl) && usersEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in usersEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "usr");
                    var dbUser = await _context.Utilizadores.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == guid);
                    if (dbUser == null)
                    {
                        var emailStr = item.TryGetProperty("email", out var em) ? em.GetString() : null;
                        if (!string.IsNullOrWhiteSpace(emailStr))
                        {
                            dbUser = await _context.Utilizadores.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email.ToLower() == emailStr.ToLower());
                        }
                    }
                    if (dbUser == null)
                    {
                        dbUser = new Utilizador
                        {
                            Id = guid,
                            EstabelecimentoId = estId,
                            CreatedAt = DateTime.UtcNow,
                            SenhaHash = "AQAAAAEAACcQAAAAEJ..."
                        };
                        _context.Utilizadores.Add(dbUser);
                    }
                    if (item.TryGetProperty("nome", out var nome)) dbUser.Nome = nome.GetString() ?? dbUser.Nome;
                    if (item.TryGetProperty("email", out var email)) dbUser.Email = email.GetString() ?? dbUser.Email;
                    if (item.TryGetProperty("perfil", out var perfil)) dbUser.Perfil = perfil.GetString() ?? dbUser.Perfil;
                    if (item.TryGetProperty("ativo", out var ativ)) dbUser.Ativo = ativ.GetBoolean();
                    if (item.TryGetProperty("restaurante_id", out var rId) && Guid.TryParse(rId.GetString(), out var rGuid))
                    {
                        dbUser.EstabelecimentoId = rGuid;
                    }
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

            // Unidades de Medida
            if (payload.TryGetProperty("unidades_medida", out var umEl) && umEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in umEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "um");
                    var dbUm = await _context.UnidadesMedida.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == guid);
                    if (dbUm == null)
                    {
                        dbUm = new UnidadeMedida { Id = guid, EstabelecimentoId = estId };
                        _context.UnidadesMedida.Add(dbUm);
                    }
                    if (item.TryGetProperty("codigo", out var cUm)) dbUm.Codigo = cUm.GetString() ?? ("UN-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("nome", out var nUm)) dbUm.Nome = nUm.GetString() ?? dbUm.Nome;
                    if (item.TryGetProperty("permite_decimal", out var pdUm)) dbUm.PermiteDecimal = pdUm.GetBoolean();
                    else if (item.TryGetProperty("permite_fracionamento", out var pfUm)) dbUm.PermiteDecimal = pfUm.GetBoolean();
                    if (item.TryGetProperty("ativo", out var atUm)) dbUm.Ativo = atUm.GetBoolean();
                }
            }

            // Armazéns
            if (payload.TryGetProperty("armazens", out var armEl) && armEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in armEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "arm");
                    var dbArm = await _context.Armazens.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == guid);
                    if (dbArm == null)
                    {
                        dbArm = new Armazem { Id = guid, EstabelecimentoId = estId };
                        _context.Armazens.Add(dbArm);
                    }
                    if (item.TryGetProperty("codigo", out var cArm)) dbArm.Codigo = cArm.GetString() ?? ("ARM-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("nome", out var nArm)) dbArm.Nome = nArm.GetString() ?? dbArm.Nome;
                    if (item.TryGetProperty("tipo", out var tArm)) dbArm.Tipo = tArm.GetString() ?? dbArm.Tipo;
                    if (item.TryGetProperty("ativo", out var atArm)) dbArm.Ativo = atArm.GetBoolean();
                }
            }

            // Locais de Armazenamento
            if (payload.TryGetProperty("locais_armazenamento", out var locEl) && locEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in locEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "loc");
                    var dbLoc = await _context.LocaisArmazenamento.FirstOrDefaultAsync(l => l.Id == guid);
                    if (dbLoc == null)
                    {
                        var armId = item.TryGetProperty("armazem_id", out var arid) && Guid.TryParse(arid.GetString(), out var arg) ? arg : estId;
                        dbLoc = new LocalArmazenamento { Id = guid, ArmazemId = armId };
                        _context.LocaisArmazenamento.Add(dbLoc);
                    }
                    if (item.TryGetProperty("tipo_estrutura", out var teLoc)) dbLoc.TipoEstrutura = teLoc.GetString() ?? dbLoc.TipoEstrutura;
                    if (item.TryGetProperty("codigo", out var cLoc)) dbLoc.Codigo = cLoc.GetString() ?? ("LOC-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("descricao", out var dLoc)) dbLoc.Descricao = dLoc.GetString();
                    if (item.TryGetProperty("ativo", out var atLoc)) dbLoc.Ativo = atLoc.GetBoolean();
                }
            }

            // Canais de Pagamento
            if (payload.TryGetProperty("canais_pagamento", out var cpEl) && cpEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in cpEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "cp");
                    var dbCp = await _context.CanaisPagamento.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == guid);
                    if (dbCp == null)
                    {
                        dbCp = new CanalPagamento { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.CanaisPagamento.Add(dbCp);
                    }
                    if (item.TryGetProperty("codigo", out var cCp)) dbCp.Codigo = cCp.GetString() ?? ("CP-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("codigo_operadora", out var coCp)) dbCp.CodigoOperadora = coCp.GetString() ?? dbCp.CodigoOperadora;
                    if (item.TryGetProperty("descricao", out var dCp)) dbCp.Descricao = dCp.GetString() ?? dbCp.Descricao;
                    if (item.TryGetProperty("numero_associado", out var nCp)) dbCp.NumeroAssociado = nCp.GetString() ?? dbCp.NumeroAssociado;
                    if (item.TryGetProperty("titular_conta", out var tcCp)) dbCp.TitularConta = tcCp.GetString();
                    if (item.TryGetProperty("instrucoes_pagamento", out var ipCp)) dbCp.InstrucoesPagamento = ipCp.GetString();
                    if (item.TryGetProperty("ativo", out var atCp)) dbCp.Ativo = atCp.GetBoolean();
                }
            }

            // Categorias de Stock
            if (payload.TryGetProperty("categorias_stock", out var cskEl) && cskEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in cskEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "csk");
                    var dbCsk = await _context.CategoriasStock.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == guid);
                    if (dbCsk == null)
                    {
                        dbCsk = new CategoriaStock { Id = guid, EstabelecimentoId = estId };
                        _context.CategoriasStock.Add(dbCsk);
                    }
                    if (item.TryGetProperty("codigo", out var cCsk)) dbCsk.Codigo = cCsk.GetString() ?? ("CAT-STK-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("nome", out var nCsk)) dbCsk.Nome = nCsk.GetString() ?? dbCsk.Nome;
                    if (item.TryGetProperty("descricao", out var dCsk)) dbCsk.Descricao = dCsk.GetString();
                }
            }

            // Caixas e Sessões
            if ((payload.TryGetProperty("caixas_sessoes", out var csEl) || payload.TryGetProperty("turnos_caixa", out csEl)) && csEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in csEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "cxs");
                    var dbCs = await _context.CaixasSessoes.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == guid);
                    if (dbCs == null)
                    {
                        dbCs = new CaixaSessao { Id = guid, EstabelecimentoId = estId, UtilizadorId = estId, AbertoEm = DateTime.UtcNow };
                        _context.CaixasSessoes.Add(dbCs);
                    }
                    if (item.TryGetProperty("codigo", out var cCs)) dbCs.Codigo = cCs.GetString() ?? ("CXS-" + guid.ToString()[..4].ToUpper());
                    else if (item.TryGetProperty("numero_turno", out var ntCs)) dbCs.Codigo = ntCs.GetString();
                    if (item.TryGetProperty("saldo_abertura", out var saCs)) dbCs.SaldoAbertura = (decimal)saCs.GetDouble();
                    else if (item.TryGetProperty("fundo_maneio_inicial", out var fmiCs)) dbCs.SaldoAbertura = (decimal)fmiCs.GetDouble();
                    if (item.TryGetProperty("saldo_fecho", out var sfCs)) dbCs.SaldoFecho = (decimal)sfCs.GetDouble();
                    else if (item.TryGetProperty("saldo_contado", out var scCs)) dbCs.SaldoFecho = (decimal)scCs.GetDouble();
                    if (item.TryGetProperty("estado", out var stCs)) dbCs.Estado = stCs.GetString() ?? dbCs.Estado;
                    if (item.TryGetProperty("observacoes", out var obsCs)) dbCs.Observacoes = obsCs.GetString();
                    else if (item.TryGetProperty("justificativa", out var justCs)) dbCs.Observacoes = justCs.GetString();
                    if (dbCs.Estado == "FECHADO" && dbCs.FechadoEm == null) dbCs.FechadoEm = DateTime.UtcNow;
                }
            }

            // Compras
            if (payload.TryGetProperty("compras", out var cmpEl) && cmpEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in cmpEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "cmp");
                    var dbCmp = await _context.Compras.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == guid);
                    if (dbCmp == null)
                    {
                        dbCmp = new Compra { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.Compras.Add(dbCmp);
                    }
                    if (item.TryGetProperty("codigo", out var cCmp)) dbCmp.Codigo = cCmp.GetString() ?? ("CMP-" + guid.ToString()[..4].ToUpper());
                    else if (item.TryGetProperty("numero_requisicao", out var nrCmp)) dbCmp.Codigo = nrCmp.GetString();
                    if (item.TryGetProperty("numero_fatura", out var nfCmp)) dbCmp.NumeroFatura = nfCmp.GetString();
                    if (item.TryGetProperty("valor_total", out var vtCmp)) dbCmp.ValorTotal = (decimal)vtCmp.GetDouble();
                    else if (item.TryGetProperty("total", out var totCmp)) dbCmp.ValorTotal = (decimal)totCmp.GetDouble();
                    if (item.TryGetProperty("status", out var stCmp)) dbCmp.Status = stCmp.GetString() ?? dbCmp.Status;
                    else if (item.TryGetProperty("estado", out var estCmp)) dbCmp.Status = estCmp.GetString() ?? dbCmp.Status;
                    if (item.TryGetProperty("observacoes", out var obsCmp)) dbCmp.Observacoes = obsCmp.GetString();
                }
            }

            // Guias de Entrada
            if ((payload.TryGetProperty("guias_entrada", out var geEl) || payload.TryGetProperty("guias", out geEl)) && geEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in geEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "ge");
                    var dbGe = await _context.GuiasEntrada.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == guid);
                    if (dbGe == null)
                    {
                        dbGe = new GuiaEntrada { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.GuiasEntrada.Add(dbGe);
                    }
                    if (item.TryGetProperty("codigo", out var cGe)) dbGe.Codigo = cGe.GetString() ?? ("GE-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("numero_guia", out var ngGe)) dbGe.NumeroGuia = ngGe.GetString() ?? dbGe.NumeroGuia;
                    else if (item.TryGetProperty("numero", out var numGe)) dbGe.NumeroGuia = numGe.GetString() ?? dbGe.NumeroGuia;
                    if (item.TryGetProperty("valor_total", out var vtGe)) dbGe.ValorTotal = (decimal)vtGe.GetDouble();
                    if (item.TryGetProperty("status", out var stGe)) dbGe.Status = stGe.GetString() ?? dbGe.Status;
                    if (item.TryGetProperty("observacoes", out var obsGe)) dbGe.Observacoes = obsGe.GetString();
                }
            }

            // Pedidos de Venda
            if ((payload.TryGetProperty("pedidos_vendas", out var pvEl) || payload.TryGetProperty("vendas", out pvEl)) && pvEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in pvEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "pv");
                    var dbPv = await _context.PedidosVendas.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == guid);
                    if (dbPv == null)
                    {
                        dbPv = new PedidoVenda { Id = guid, EstabelecimentoId = estId, CreatedAt = DateTime.UtcNow };
                        _context.PedidosVendas.Add(dbPv);
                    }
                    if (item.TryGetProperty("codigo", out var cPv)) dbPv.Codigo = cPv.GetString() ?? ("PV-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("numero_pedido", out var npPv)) dbPv.NumeroPedido = npPv.GetString() ?? dbPv.NumeroPedido;
                    else if (item.TryGetProperty("numero", out var numVnd)) dbPv.NumeroPedido = numVnd.GetString() ?? dbPv.NumeroPedido;
                    if (item.TryGetProperty("cliente_nome", out var cnPv)) dbPv.ClienteNome = cnPv.GetString();
                    if (item.TryGetProperty("cliente_telefone", out var ctPv)) dbPv.ClienteTelefone = ctPv.GetString();
                    if (item.TryGetProperty("numero_mesa", out var nmPv)) dbPv.NumeroMesa = nmPv.GetString();
                    else if (item.TryGetProperty("mesa_local", out var mlPv)) dbPv.NumeroMesa = mlPv.GetString();
                    if (item.TryGetProperty("metodo_pagamento", out var mpPv)) dbPv.MetodoPagamento = mpPv.GetString() ?? dbPv.MetodoPagamento;
                    if (item.TryGetProperty("subtotal", out var sbPv)) dbPv.Subtotal = (decimal)sbPv.GetDouble();
                    else if (item.TryGetProperty("base_tributavel", out var btPv)) dbPv.Subtotal = (decimal)btPv.GetDouble();
                    if (item.TryGetProperty("valor_total", out var vtPv)) dbPv.ValorTotal = (decimal)vtPv.GetDouble();
                    else if (item.TryGetProperty("total", out var totVnd)) dbPv.ValorTotal = (decimal)totVnd.GetDouble();
                    if (item.TryGetProperty("estado", out var stPv)) dbPv.Estado = stPv.GetString() ?? dbPv.Estado;
                    if (item.TryGetProperty("observacoes", out var obsPv)) dbPv.Observacoes = obsPv.GetString();
                }
            }

            // Pedidos Online
            if (payload.TryGetProperty("pedidos_online", out var poEl) && poEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in poEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "po");
                    var dbPo = await _context.PedidosOnline.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == guid);
                    if (dbPo == null)
                    {
                        dbPo = new PedidoOnline { Id = guid, EstabelecimentoId = estId, DataPedido = DateTime.UtcNow };
                        _context.PedidosOnline.Add(dbPo);
                    }
                    if (item.TryGetProperty("codigo", out var cPo)) dbPo.Codigo = cPo.GetString() ?? ("POL-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("numero_pedido", out var npPo)) dbPo.NumeroPedido = npPo.GetString() ?? dbPo.NumeroPedido;
                    if (item.TryGetProperty("cliente_nome", out var cnPo)) dbPo.ClienteNome = cnPo.GetString() ?? dbPo.ClienteNome;
                    if (item.TryGetProperty("cliente_telefone", out var ctPo)) dbPo.ClienteTelefone = ctPo.GetString() ?? dbPo.ClienteTelefone;
                    if (item.TryGetProperty("tipo_entrega", out var tePo)) dbPo.TipoEntrega = tePo.GetString() ?? dbPo.TipoEntrega;
                    if (item.TryGetProperty("numero_mesa", out var nmPo)) dbPo.NumeroMesa = nmPo.GetString();
                    if (item.TryGetProperty("endereco_entrega", out var eePo)) dbPo.EnderecoEntrega = eePo.GetString();
                    if (item.TryGetProperty("operadora_pagamento", out var opPo)) dbPo.OperadoraPagamento = opPo.GetString() ?? dbPo.OperadoraPagamento;
                    if (item.TryGetProperty("referencia_pagamento", out var rpPo)) dbPo.ReferenciaPagamento = rpPo.GetString();
                    if (item.TryGetProperty("subtotal", out var sbPo)) dbPo.Subtotal = (decimal)sbPo.GetDouble();
                    if (item.TryGetProperty("total", out var totPo)) dbPo.Total = (decimal)totPo.GetDouble();
                    if (item.TryGetProperty("estado", out var stPo)) dbPo.Estado = stPo.GetString() ?? dbPo.Estado;
                    if (item.TryGetProperty("stock_deduzido", out var sdPo)) dbPo.StockDeduzido = sdPo.GetBoolean();
                    if (item.TryGetProperty("validado_por", out var vpPo)) dbPo.ValidadoPor = vpPo.GetString();
                }
            }

            // Itens de Compra
            if (payload.TryGetProperty("itens_compra", out var itCmpEl) && itCmpEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in itCmpEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "itcmp");
                    var dbItCmp = await _context.ItensCompra.FirstOrDefaultAsync(ic => ic.Id == guid);
                    if (dbItCmp == null)
                    {
                        dbItCmp = new ItemCompra { Id = guid };
                        _context.ItensCompra.Add(dbItCmp);
                    }
                    if (item.TryGetProperty("codigo", out var codItCmp)) dbItCmp.Codigo = codItCmp.GetString() ?? ("IC-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("compra_id", out var cmpId) && Guid.TryParse(cmpId.GetString(), out var cmpG)) dbItCmp.CompraId = cmpG;
                    if (item.TryGetProperty("item_stock_id", out var stkId) && Guid.TryParse(stkId.GetString(), out var stkG)) dbItCmp.ItemStockId = stkG;
                    if (item.TryGetProperty("quantidade", out var qtdItCmp)) dbItCmp.Quantidade = (decimal)qtdItCmp.GetDouble();
                    if (item.TryGetProperty("preco_unitario", out var prItCmp)) dbItCmp.PrecoUnitario = (decimal)prItCmp.GetDouble();
                }
            }

            // Itens de Guia de Entrada
            if ((payload.TryGetProperty("itens_guia_entrada", out var itGeEl) || payload.TryGetProperty("itens_guias_entrada", out itGeEl)) && itGeEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in itGeEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "itge");
                    var dbItGe = await _context.ItensGuiaEntrada.FirstOrDefaultAsync(ig => ig.Id == guid);
                    if (dbItGe == null)
                    {
                        dbItGe = new ItemGuiaEntrada { Id = guid };
                        _context.ItensGuiaEntrada.Add(dbItGe);
                    }
                    if (item.TryGetProperty("codigo", out var codItGe)) dbItGe.Codigo = codItGe.GetString() ?? ("IGE-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("guia_entrada_id", out var geId) && Guid.TryParse(geId.GetString(), out var geG)) dbItGe.GuiaEntradaId = geG;
                    if (item.TryGetProperty("item_stock_id", out var stkId2) && Guid.TryParse(stkId2.GetString(), out var stkG2)) dbItGe.ItemStockId = stkG2;
                    if (item.TryGetProperty("quantidade", out var qtdItGe)) dbItGe.Quantidade = (decimal)qtdItGe.GetDouble();
                    if (item.TryGetProperty("preco_custo", out var pcItGe)) dbItGe.PrecoCusto = (decimal)pcItGe.GetDouble();
                }
            }

            // Itens de Pedidos de Venda
            if ((payload.TryGetProperty("itens_pedidos_vendas", out var itPvEl) || payload.TryGetProperty("itens_pedido_venda", out itPvEl)) && itPvEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in itPvEl.EnumerateArray())
                {
                    var idStr = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                    var guid = ToGuid(idStr, "itpv");
                    var dbItPv = await _context.ItensPedidoVenda.FirstOrDefaultAsync(ipv => ipv.Id == guid);
                    if (dbItPv == null)
                    {
                        dbItPv = new ItemPedidoVenda { Id = guid };
                        _context.ItensPedidoVenda.Add(dbItPv);
                    }
                    if (item.TryGetProperty("codigo", out var codItPv)) dbItPv.Codigo = codItPv.GetString() ?? ("IPV-" + guid.ToString()[..4].ToUpper());
                    if (item.TryGetProperty("pedido_id", out var pedId) && Guid.TryParse(pedId.GetString(), out var pedG)) dbItPv.PedidoId = pedG;
                    if (item.TryGetProperty("produto_id", out var prodId) && Guid.TryParse(prodId.GetString(), out var prodG)) dbItPv.ProdutoId = prodG;
                    if (item.TryGetProperty("quantidade", out var qtdItPv)) dbItPv.Quantidade = (decimal)qtdItPv.GetDouble();
                    if (item.TryGetProperty("preco_unitario", out var prItPv)) dbItPv.PrecoUnitario = (decimal)prItPv.GetDouble();
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
