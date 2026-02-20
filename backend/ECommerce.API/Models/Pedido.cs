namespace ECommerce.API.Models;

public enum StatusPedido
{
    Criado = 0,
    Pago = 1,
    Cancelado = 2
}

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public StatusPedido Status { get; set; } = StatusPedido.Criado;
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }

    public ICollection<PedidoProduto> Itens { get; set; } = new List<PedidoProduto>();

    public decimal ValorTotal => Itens.Sum(i => i.PrecoUnitario * i.Quantidade);

    public bool PodeSerCancelado() => Status == StatusPedido.Criado;

    public bool PodeSerPago() => Status == StatusPedido.Criado;
}

public class PedidoProduto
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; } = null!;
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
