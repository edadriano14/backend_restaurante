using Microsoft.EntityFrameworkCore;
using RedeRestaurante.API.Models;
using RedeRestaurante.API.Services;

namespace RedeRestaurante.API.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Estabelecimento> Estabelecimentos => Set<Estabelecimento>();
    public DbSet<Utilizador> Utilizadores => Set<Utilizador>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<CategoriaStock> CategoriasStock => Set<CategoriaStock>();
    public DbSet<ItemStock> ItensStock => Set<ItemStock>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItensCompra => Set<ItemCompra>();
    public DbSet<Cliente> Clientes => Set<Cliente>();

    // NOVOS MODELOS
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<CaixaSessao> CaixasSessoes => Set<CaixaSessao>();
    public DbSet<PedidoVenda> PedidosVendas => Set<PedidoVenda>();
    public DbSet<ItemPedidoVenda> ItensPedidoVenda => Set<ItemPedidoVenda>();
    public DbSet<GuiaEntrada> GuiasEntrada => Set<GuiaEntrada>();
    public DbSet<ItemGuiaEntrada> ItensGuiaEntrada => Set<ItemGuiaEntrada>();

    // TABELAS RELACIONAIS DE APOIO
    public DbSet<UnidadeMedida> UnidadesMedida => Set<UnidadeMedida>();
    public DbSet<Armazem> Armazens => Set<Armazem>();
    public DbSet<LocalArmazenamento> LocaisArmazenamento => Set<LocalArmazenamento>();
    public DbSet<MetodoPagamento> MetodosPagamento => Set<MetodoPagamento>();
    public DbSet<Acompanhamento> Acompanhamentos => Set<Acompanhamento>();
    public DbSet<PostoPreparacao> PostosPreparacao => Set<PostoPreparacao>();
    public DbSet<TipoPedido> TiposPedidos => Set<TipoPedido>();
    public DbSet<Sala> Salas => Set<Sala>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<CanalPagamento> CanaisPagamento => Set<CanalPagamento>();
    public DbSet<PedidoOnline> PedidosOnline => Set<PedidoOnline>();
    public DbSet<TaxaIva> TaxasIva => Set<TaxaIva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ISOLAMENTO MULTI-TENANT AUTOMÁTICO
        modelBuilder.Entity<Utilizador>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Fornecedor>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<CategoriaStock>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<ItemStock>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Compra>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Cliente>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Categoria>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Produto>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<CaixaSessao>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<PedidoVenda>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<GuiaEntrada>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<UnidadeMedida>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Armazem>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<MetodoPagamento>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Acompanhamento>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<PostoPreparacao>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<TipoPedido>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Sala>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<Mesa>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<CanalPagamento>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<PedidoOnline>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);

        modelBuilder.Entity<TaxaIva>()
            .HasQueryFilter(e => !_tenantService.EstabelecimentoId.HasValue || e.EstabelecimentoId == _tenantService.EstabelecimentoId.Value);
    }
}
