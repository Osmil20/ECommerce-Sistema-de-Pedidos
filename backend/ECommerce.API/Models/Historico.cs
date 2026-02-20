namespace ECommerce.API.Models;

public enum TipoEntidade
{
    Cliente,
    Produto,
    Pedido,
    Usuario
}

public enum TipoOperacao
{
    Criacao,
    Atualizacao,
    Cancelamento,
    Pagamento
}

public class Historico
{
    public int Id { get; set; }
    public TipoEntidade Entidade { get; set; }
    public int EntidadeId { get; set; }
    public TipoOperacao Operacao { get; set; }
    public string DadosAnteriores { get; set; } = string.Empty;
    public string DadosNovos { get; set; } = string.Empty;
    public DateTime RealizadoEm { get; set; } = DateTime.UtcNow;
    public string RealizadoPor { get; set; } = "Sistema";
}
