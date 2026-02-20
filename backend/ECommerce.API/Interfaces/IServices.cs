using ECommerce.API.DTOs;
using ECommerce.API.Models;

namespace ECommerce.API.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDto>> ListarAsync();
    Task<ClienteResponseDto?> ObterPorIdAsync(int id);
    Task<ClienteResponseDto> CriarAsync(ClienteCreateDto dto);
    Task<ClienteResponseDto?> AtualizarAsync(int id, ClienteUpdateDto dto);
    Task<bool> RemoverAsync(int id);
}

public interface IProdutoService
{
    Task<IEnumerable<ProdutoResponseDto>> ListarAsync();
    Task<ProdutoResponseDto?> ObterPorIdAsync(int id);
    Task<ProdutoResponseDto> CriarAsync(ProdutoCreateDto dto);
    Task<ProdutoResponseDto?> AtualizarAsync(int id, ProdutoUpdateDto dto);
    Task<bool> RemoverAsync(int id);
}

public interface IPedidoService
{
    Task<IEnumerable<PedidoResponseDto>> ListarAsync(StatusPedido? status = null);
    Task<PedidoResponseDto?> ObterPorIdAsync(int id);
    Task<PedidoResponseDto> CriarAsync(PedidoCreateDto dto);
    Task<PedidoResponseDto?> PagarAsync(int id);
    Task<PedidoResponseDto?> CancelarAsync(int id);
}

public interface IHistoricoService
{
    Task<IEnumerable<HistoricoResponseDto>> ListarPorEntidadeAsync(TipoEntidade entidade, int entidadeId);
    Task RegistrarAsync(TipoEntidade entidade, int entidadeId, TipoOperacao operacao, string dadosAnteriores, string dadosNovos, string realizadoPor = "Sistema");
}
