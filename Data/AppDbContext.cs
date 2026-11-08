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
    public DbSet<TabelaPrecoFornecedor> TabelaPrecosFornecedores => Set<TabelaPrecoFornecedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ISOLAMENTO MULTI-TENANT AUTOMÁTICO
        // Quando EstabelecimentoId for null (acesso global ou sem tenant especificado), não filtra por tenant
        modelBuilder.Entity<Utilizador>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Fornecedor>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<CategoriaStock>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<ItemStock>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Compra>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Cliente>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Categoria>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Produto>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<CaixaSessao>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<PedidoVenda>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<GuiaEntrada>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<UnidadeMedida>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Armazem>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<MetodoPagamento>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Acompanhamento>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<PostoPreparacao>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<TipoPedido>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Sala>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<Mesa>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<CanalPagamento>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<PedidoOnline>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<TaxaIva>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        modelBuilder.Entity<TabelaPrecoFornecedor>()
            .HasQueryFilter(e => _tenantService.EstabelecimentoId == null || e.EstabelecimentoId == _tenantService.EstabelecimentoId);

        // Mapeamento explícito da chave estrangeira de Itens do Pedido de Venda
        modelBuilder.Entity<PedidoVenda>()
            .HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Compra>()
            .HasMany(c => c.Itens)
            .WithOne()
            .HasForeignKey(i => i.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GuiaEntrada>()
            .HasMany(g => g.Itens)
            .WithOne()
            .HasForeignKey(i => i.GuiaEntradaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
