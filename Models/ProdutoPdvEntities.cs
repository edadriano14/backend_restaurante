using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedeRestaurante.API.Models;

[Table("categorias")]
public class Categoria
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [MaxLength(20)]
    [Column("emoji")]
    public string Emoji { get; set; } = "🍽️";

    [Column("ordem")]
    public int Ordem { get; set; } = 0;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Produto> Produtos { get; set; } = new();
}

[Table("produtos")]
public class Produto
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Column("categoria_id")]
    public Guid? CategoriaId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo_produto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("preco")]
    public decimal Preco { get; set; } = 0;

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "disponivel"; // disponivel, indisponivel

    [Column("imagem_url")]
    public string? ImagemUrl { get; set; }

    [Column("grupo_opcoes_acompanhamento", TypeName = "jsonb")]
    public string GrupoOpcoesAcompanhamento { get; set; } = "[]";

    [Column("tem_promocao")]
    public bool TemPromocao { get; set; } = false;

    [Column("preco_promocional")]
    public decimal? PrecoPromocional { get; set; }

    [Column("hora_inicio_promo")]
    public TimeSpan? HoraInicioPromo { get; set; }

    [Column("hora_fim_promo")]
    public TimeSpan? HoraFimPromo { get; set; }

    [MaxLength(50)]
    [Column("dias_semana_promo")]
    public string DiasSemanaPromo { get; set; } = "1,2,3,4,5,6,7";

    [Column("em_destaque")]
    public bool EmDestaque { get; set; } = false;

    [Column("total_comprados")]
    public int TotalComprados { get; set; } = 0;

    [Column("item_stock_id")]
    public Guid? ItemStockId { get; set; }

    [Column("controla_stock")]
    public bool ControlaStock { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }

    [ForeignKey("ItemStockId")]
    public ItemStock? ItemStock { get; set; }
}

[Table("caixas_sessoes")]
public class CaixaSessao
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Column("utilizador_id")]
    public Guid UtilizadorId { get; set; }

    [Column("saldo_abertura")]
    public decimal SaldoAbertura { get; set; } = 0;

    [Column("saldo_fecho")]
    public decimal? SaldoFecho { get; set; }

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "ABERTO";

    [Column("aberto_em")]
    public DateTime AbertoEm { get; set; } = DateTime.UtcNow;

    [Column("fechado_em")]
    public DateTime? FechadoEm { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }
}

[Table("pedidos_vendas")]
public class PedidoVenda
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Column("caixa_sessao_id")]
    public Guid? CaixaSessaoId { get; set; }

    [Column("cliente_id")]
    public Guid? ClienteId { get; set; }

    [Column("sala_id")]
    public Guid? SalaId { get; set; }

    [Column("mesa_id")]
    public Guid? MesaId { get; set; }

    [MaxLength(20)]
    [Column("numero_mesa")]
    public string? NumeroMesa { get; set; }

    [MaxLength(30)]
    [Column("tipo_origem")]
    public string TipoOrigem { get; set; } = "PDV_BALCAO"; // PDV_BALCAO, PORTAL_CLIENTE_ONLINE, MESA

    [MaxLength(50)]
    [Column("numero_pedido")]
    public string NumeroPedido { get; set; } = string.Empty;

    [MaxLength(150)]
    [Column("cliente_nome")]
    public string? ClienteNome { get; set; }

    [MaxLength(30)]
    [Column("cliente_telefone")]
    public string? ClienteTelefone { get; set; }

    [Column("endereco_entrega")]
    public string? EnderecoEntrega { get; set; }

    [MaxLength(50)]
    [Column("metodo_pagamento")]
    public string MetodoPagamento { get; set; } = "DINHEIRO";

    [Column("subtotal")]
    public decimal Subtotal { get; set; } = 0;

    [Column("taxa_entrega")]
    public decimal TaxaEntrega { get; set; } = 0;

    [Column("desconto")]
    public decimal Desconto { get; set; } = 0;

    [Column("valor_total")]
    public decimal ValorTotal { get; set; } = 0;

    [MaxLength(30)]
    [Column("estado")]
    public string Estado { get; set; } = "PENDENTE"; // PENDENTE, EM_PREPARO, PRONTO, ENTREGUE, CANCELADO

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ItemPedidoVenda> Itens { get; set; } = new();
}

[Table("itens_pedido_venda")]
public class ItemPedidoVenda
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("pedido_id")]
    public Guid PedidoId { get; set; }

    [Column("produto_id")]
    public Guid ProdutoId { get; set; }

    [Column("quantidade")]
    public decimal Quantidade { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("opcoes_selecionadas")]
    public string? OpcoesSelecionadas { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    [ForeignKey("ProdutoId")]
    public Produto? Produto { get; set; }
}

[Table("guias_entrada")]
public class GuiaEntrada
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Column("fornecedor_id")]
    public Guid? FornecedorId { get; set; }

    [MaxLength(100)]
    [Column("numero_guia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [Column("data_emissao")]
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

    [Column("valor_total")]
    public decimal ValorTotal { get; set; } = 0;

    [MaxLength(30)]
    [Column("status")]
    public string Status { get; set; } = "PROCESSADA";

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ItemGuiaEntrada> Itens { get; set; } = new();
}

[Table("itens_guia_entrada")]
public class ItemGuiaEntrada
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("guia_entrada_id")]
    public Guid GuiaEntradaId { get; set; }

    [Column("item_stock_id")]
    public Guid ItemStockId { get; set; }

    [Column("quantidade")]
    public decimal Quantidade { get; set; }

    [Column("preco_custo")]
    public decimal PrecoCusto { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    [ForeignKey("ItemStockId")]
    public ItemStock? ItemStock { get; set; }
}

[Table("metodos_pagamento")]
public class MetodoPagamento
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("icone")]
    public string Icone { get; set; } = "fa-solid fa-money-bill";

    [Column("taxa_percentual")]
    public decimal TaxaPercentual { get; set; } = 0;

    [Column("permite_troco")]
    public bool PermiteTroco { get; set; } = true;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("ordem")]
    public int Ordem { get; set; } = 0;
}

[Table("acompanhamentos")]
public class Acompanhamento
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("preco_adicional")]
    public decimal PrecoAdicional { get; set; } = 0;

    [MaxLength(30)]
    [Column("tipo_selecao")]
    public string TipoSelecao { get; set; } = "opcional_pago"; // obrigatorio, opcional_pago

    [MaxLength(100)]
    [Column("grupo_selecao")]
    public string GrupoSelecao { get; set; } = "Guarnições";

    [Column("maximo_selecao")]
    public int MaximoSelecao { get; set; } = 1;

    [Column("disponivel")]
    public bool Disponivel { get; set; } = true;
}

[Table("postos_preparacao")]
public class PostoPreparacao
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}

[Table("tipos_pedidos")]
public class TipoPedido
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}

[Table("salas")]
public class Sala
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("capacidade_mesas")]
    public int CapacidadeMesas { get; set; } = 10;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    public List<Mesa> Mesas { get; set; } = new();
}

[Table("mesas")]
public class Mesa
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Column("sala_id")]
    public Guid SalaId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("numero_mesa")]
    public string NumeroMesa { get; set; } = string.Empty;

    [Column("capacidade_lugares")]
    public int CapacidadeLugares { get; set; } = 4;

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "LIVRE"; // LIVRE, OCUPADA, RESERVADA

    [MaxLength(100)]
    [Column("qrcode_token")]
    public string? QrCodeToken { get; set; }

    [Column("qrcode_url")]
    public string? QrCodeUrl { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [ForeignKey("SalaId")]
    public Sala? Sala { get; set; }
}

[Table("pedidos_online")]
public class PedidoOnline
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Column("pedido_venda_id")]
    public Guid? PedidoVendaId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("numero_pedido")]
    public string NumeroPedido { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("cliente_nome")]
    public string ClienteNome { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("cliente_telefone")]
    public string ClienteTelefone { get; set; } = string.Empty;

    [MaxLength(30)]
    [Column("tipo_entrega")]
    public string TipoEntrega { get; set; } = "DELIVERY"; // DELIVERY, TAKEAWAY, MESA

    [Column("mesa_id")]
    public Guid? MesaId { get; set; }

    [MaxLength(30)]
    [Column("numero_mesa")]
    public string? NumeroMesa { get; set; }

    [Column("endereco_entrega")]
    public string? EnderecoEntrega { get; set; }

    [MaxLength(50)]
    [Column("operadora_pagamento")]
    public string OperadoraPagamento { get; set; } = "MPESA";

    [MaxLength(100)]
    [Column("referencia_pagamento")]
    public string? ReferenciaPagamento { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; } = 0;

    [Column("taxa_entrega")]
    public decimal TaxaEntrega { get; set; } = 0;

    [Column("total")]
    public decimal Total { get; set; } = 0;

    [MaxLength(30)]
    [Column("estado")]
    public string Estado { get; set; } = "PENDENTE_VALIDACAO"; // PENDENTE_VALIDACAO, VALIDADO_EM_PREPARO, PRONTO, ENTREGUE, REJEITADO

    [Column("stock_deduzido")]
    public bool StockDeduzido { get; set; } = false;

    [Column("data_pedido")]
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    [Column("validado_por")]
    public string? ValidadoPor { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [ForeignKey("PedidoVendaId")]
    public PedidoVenda? PedidoVenda { get; set; }

    [ForeignKey("MesaId")]
    public Mesa? Mesa { get; set; }
}

[Table("taxas_iva")]
public class TaxaIva
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("estabelecimento_id")]
    public Guid? EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Column("iva")]
    public decimal? Iva { get; set; } = 16.00m;

    [Required]
    [MaxLength(10)]
    [Column("moeda")]
    public string Moeda { get; set; } = "MZN";

    [MaxLength(120)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("padrao")]
    public bool Padrao { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
