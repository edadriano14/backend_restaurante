using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedeRestaurante.API.Models;

[Table("estabelecimentos")]
public class Estabelecimento
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [Column("logotipo_url")]
    public string? LogotipoUrl { get; set; }

    [MaxLength(7)]
    [Column("cor_primaria")]
    public string CorPrimaria { get; set; } = "#E53935";

    [MaxLength(7)]
    [Column("cor_secundaria")]
    public string CorSecundaria { get; set; } = "#1E293B";

    [MaxLength(30)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [Column("endereco")]
    public string? Endereco { get; set; }

    [MaxLength(10)]
    [Column("moeda")]
    public string Moeda { get; set; } = "MZN";

    [MaxLength(50)]
    [Column("horario_abertura")]
    public string HorarioAbertura { get; set; } = "08:00 - 23:00";

    [MaxLength(100)]
    [Column("dias_funcionamento")]
    public string DiasFuncionamento { get; set; } = "Terça a Domingo";

    [MaxLength(30)]
    [Column("whatsapp")]
    public string? Whatsapp { get; set; }

    [MaxLength(150)]
    [Column("facebook")]
    public string? Facebook { get; set; }

    [MaxLength(150)]
    [Column("instagram")]
    public string? Instagram { get; set; }

    [MaxLength(150)]
    [Column("website")]
    public string? Website { get; set; }

    [MaxLength(100)]
    [Column("cidade")]
    public string Cidade { get; set; } = "Maputo";

    [Column("descricao_rodape")]
    public string? DescricaoRodape { get; set; }

    [Column("tem_piscina")]
    public bool TemPiscina { get; set; } = false;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[Table("canais_pagamento")]
public class CanalPagamento
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
    [MaxLength(50)]
    [Column("codigo_operadora")]
    public string CodigoOperadora { get; set; } = string.Empty; // MPESA, EMOLA, CONTA_MOVEL

    [Required]
    [MaxLength(100)]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("numero_associado")]
    public string NumeroAssociado { get; set; } = string.Empty; // +258 84 000 0000

    [MaxLength(150)]
    [Column("titular_conta")]
    public string? TitularConta { get; set; }

    [Column("instrucoes_pagamento")]
    public string? InstrucoesPagamento { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

[Table("utilizadores")]
public class Utilizador
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("auth_user_id")]
    public Guid? AuthUserId { get; set; }

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(120)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("senha_hash")]
    public string SenhaHash { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("perfil")]
    public string Perfil { get; set; } = "OPERADOR"; // ADMIN_GERAL, GESTOR, OPERADOR, STOCK

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("EstabelecimentoId")]
    public Estabelecimento? Estabelecimento { get; set; }
}

[Table("fornecedores")]
public class Fornecedor
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
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("nif")]
    public string? Nif { get; set; }

    [MaxLength(30)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(100)]
    [Column("contacto_responsavel")]
    public string? ContactoResponsavel { get; set; }

    [Column("morada")]
    public string? Morada { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

[Table("categorias_stock")]
public class CategoriaStock
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

    [Column("descricao")]
    public string? Descricao { get; set; }
}

[Table("itens_stock")]
public class ItemStock
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
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("codigo_barras")]
    public string? CodigoBarras { get; set; }

    [MaxLength(20)]
    [Column("unidade_medida")]
    public string UnidadeMedida { get; set; } = "UN"; // KG, UN, LT, CX

    [Column("unidade_medida_id")]
    public Guid? UnidadeMedidaId { get; set; }

    [Column("armazem_id")]
    public Guid? ArmazemId { get; set; }

    [Column("local_armazenamento_id")]
    public Guid? LocalArmazenamentoId { get; set; }

    [Column("quantidade_atual")]
    public decimal QuantidadeAtual { get; set; } = 0;

    [Column("quantidade_reservada")]
    public decimal QuantidadeReservada { get; set; } = 0;

    [Column("quantidade_minima")]
    public decimal QuantidadeMinima { get; set; } = 5;

    [Column("custo_unitario")]
    public decimal CustoUnitario { get; set; } = 0;

    [Column("disponivel_pdv")]
    public bool DisponivelPdv { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CategoriaId")]
    public CategoriaStock? Categoria { get; set; }

    [ForeignKey("UnidadeMedidaId")]
    public UnidadeMedida? UnidadeMedidaObj { get; set; }

    [ForeignKey("ArmazemId")]
    public Armazem? Armazem { get; set; }

    [ForeignKey("LocalArmazenamentoId")]
    public LocalArmazenamento? LocalArmazenamento { get; set; }
}

[Table("unidades_medida")]
public class UnidadeMedida
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("estabelecimento_id")]
    public Guid EstabelecimentoId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("permite_decimal")]
    public bool PermiteDecimal { get; set; } = false;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}

[Table("armazens")]
public class Armazem
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
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("tipo")]
    public string Tipo { get; set; } = "GERAL"; // GERAL, COZINHA, BAR, PASTELARIA

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    public List<LocalArmazenamento> Locais { get; set; } = new();
}

[Table("locais_armazenamento")]
public class LocalArmazenamento
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("armazem_id")]
    public Guid ArmazemId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo_estrutura")]
    public string TipoEstrutura { get; set; } = "ESTANTE"; // ESTANTE, PRATELEIRA, GELEIRA, CONGELADOR

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [MaxLength(150)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [ForeignKey("ArmazemId")]
    public Armazem? Armazem { get; set; }
}

[Table("compras")]
public class Compra
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
    [Column("numero_fatura")]
    public string? NumeroFatura { get; set; }

    [Column("data_compra")]
    public DateTime DataCompra { get; set; } = DateTime.UtcNow;

    [Column("valor_total")]
    public decimal ValorTotal { get; set; } = 0;

    [MaxLength(30)]
    [Column("status")]
    public string Status { get; set; } = "CONCLUIDA";

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FornecedorId")]
    public Fornecedor? Fornecedor { get; set; }

    public List<ItemCompra> Itens { get; set; } = new();
}

[Table("itens_compra")]
public class ItemCompra
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Column("compra_id")]
    public Guid CompraId { get; set; }

    [Column("item_stock_id")]
    public Guid ItemStockId { get; set; }

    [Column("quantidade")]
    public decimal Quantidade { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    [ForeignKey("ItemStockId")]
    public ItemStock? ItemStock { get; set; }
}

[Table("clientes")]
public class Cliente
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
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(30)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [MaxLength(120)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(50)]
    [Column("nif")]
    public string? Nif { get; set; }

    [Column("morada")]
    public string? Morada { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
