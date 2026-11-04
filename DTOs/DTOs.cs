namespace RedeRestaurante.API.DTOs;

public record LoginRequest(string Email, string Senha);

public record LoginResponse(
    string Token,
    Guid UtilizadorId,
    string Nome,
    string Email,
    string Perfil,
    Guid EstabelecimentoId,
    string EstabelecimentoNome,
    string CorPrimaria,
    string CorSecundaria,
    string? LogotipoUrl
);

public record EstabelecimentoUpdateRequest(
    string Nome,
    string? LogotipoUrl,
    string CorPrimaria,
    string CorSecundaria,
    string? Telefone,
    string? Email,
    string? Endereco,
    string? Moeda
);

public record ItemStockDto(
    Guid? Id,
    string Nome,
    Guid? CategoriaId,
    string? CodigoBarras,
    string UnidadeMedida,
    decimal QuantidadeAtual,
    decimal QuantidadeMinima,
    decimal CustoUnitario
);

public record FornecedorDto(
    Guid? Id,
    string Nome,
    string? Nif,
    string? Telefone,
    string? Email,
    string? ContactoResponsavel,
    string? Morada
);

public record ClienteDto(
    Guid? Id,
    string Nome,
    string? Telefone,
    string? Email,
    string? Nif,
    string? Morada,
    string? Observacoes
);

public record ItemCompraInputDto(
    Guid ItemStockId,
    decimal Quantidade,
    decimal PrecoUnitario
);

public record CompraCreateDto(
    Guid? FornecedorId,
    string? NumeroFatura,
    string? Observacoes,
    List<ItemCompraInputDto> Itens
);

public record UtilizadorDto(
    Guid? Id,
    string Nome,
    string Email,
    string? Senha,
    string Perfil,
    bool Ativo
);
