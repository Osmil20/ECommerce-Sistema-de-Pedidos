namespace ECommerce.API.DTOs;

// ---- Cliente ----
public record ClienteCreateDto(string Nome, string Cpf, string Email);
public record ClienteUpdateDto(string Nome, string Email);
public record ClienteResponseDto(int Id, string Nome, string Cpf, string Email, DateTime CriadoEm);

// ---- Produto ----
public record ProdutoCreateDto(string Nome, decimal Preco, string Descricao, int Estoque);
public record ProdutoUpdateDto(string Nome, decimal Preco, string Descricao, int Estoque);
public record ProdutoResponseDto(int Id, string Nome, decimal Preco, string Descricao, int Estoque);

// ---- Pedido ----
public record ItemPedidoDto(int ProdutoId, int Quantidade);

public record PedidoCreateDto(int ClienteId, List<ItemPedidoDto> Itens);

public record PedidoItemResponseDto(
    int ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal);

public record PedidoResponseDto(
    int Id,
    int ClienteId,
    string NomeCliente,
    string Status,
    DateTime DataPedido,
    DateTime? DataAtualizacao,
    decimal ValorTotal,
    List<PedidoItemResponseDto> Itens);

// ---- Histórico ----
public record HistoricoResponseDto(
    int Id,
    string Entidade,
    int EntidadeId,
    string Operacao,
    string DadosAnteriores,
    string DadosNovos,
    DateTime RealizadoEm,
    string RealizadoPor);
