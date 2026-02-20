using System.Text.Json;
using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _context;
    private readonly IHistoricoService _historico;

    public PedidoService(AppDbContext context, IHistoricoService historico)
    {
        _context = context;
        _historico = historico;
    }

    public async Task<IEnumerable<PedidoResponseDto>> ListarAsync(StatusPedido? status = null)
    {
        var query = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var pedidos = await query.OrderByDescending(p => p.DataPedido).ToListAsync();
        return pedidos.Select(ToDto);
    }

    public async Task<PedidoResponseDto?> ObterPorIdAsync(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido is null ? null : ToDto(pedido);
    }

    public async Task<PedidoResponseDto> CriarAsync(PedidoCreateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(dto.ClienteId)
            ?? throw new InvalidOperationException($"Cliente {dto.ClienteId} não encontrado.");

        if (!dto.Itens.Any())
            throw new InvalidOperationException("O pedido deve ter ao menos um produto.");

        var produtoIds = dto.Itens.Select(i => i.ProdutoId).Distinct().ToList();
        var produtos = await _context.Produtos
            .Where(p => produtoIds.Contains(p.Id))
            .ToListAsync();

        if (produtos.Count != produtoIds.Count)
            throw new InvalidOperationException("Um ou mais produtos não foram encontrados.");

        var pedido = new Pedido
        {
            ClienteId = dto.ClienteId,
            Cliente = cliente,
            Status = StatusPedido.Criado,
            DataPedido = DateTime.UtcNow
        };

        foreach (var item in dto.Itens)
        {
            var produto = produtos.First(p => p.Id == item.ProdutoId);

            if (produto.Estoque < item.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para o produto '{produto.Nome}'.");

            pedido.Itens.Add(new PedidoProduto
            {
                ProdutoId = produto.Id,
                Produto = produto,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco
            });

            produto.Estoque -= item.Quantidade;
        }

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Pedido,
            pedido.Id,
            TipoOperacao.Criacao,
            string.Empty,
            JsonSerializer.Serialize(new { pedido.Id, pedido.ClienteId, Status = pedido.Status.ToString() }));

        return ToDto(pedido);
    }

    public async Task<PedidoResponseDto?> PagarAsync(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null) return null;

        if (!pedido.PodeSerPago())
            throw new InvalidOperationException($"Pedido com status '{pedido.Status}' não pode ser pago.");

        var statusAnterior = pedido.Status.ToString();
        pedido.Status = StatusPedido.Pago;
        pedido.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Pedido,
            pedido.Id,
            TipoOperacao.Pagamento,
            JsonSerializer.Serialize(new { Status = statusAnterior }),
            JsonSerializer.Serialize(new { Status = StatusPedido.Pago.ToString() }));

        return ToDto(pedido);
    }

    public async Task<PedidoResponseDto?> CancelarAsync(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null) return null;

        if (!pedido.PodeSerCancelado())
            throw new InvalidOperationException($"Pedido com status '{pedido.Status}' não pode ser cancelado.");

        var statusAnterior = pedido.Status.ToString();
        pedido.Status = StatusPedido.Cancelado;
        pedido.DataAtualizacao = DateTime.UtcNow;

        // Devolver estoque
        foreach (var item in pedido.Itens)
            item.Produto.Estoque += item.Quantidade;

        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Pedido,
            pedido.Id,
            TipoOperacao.Cancelamento,
            JsonSerializer.Serialize(new { Status = statusAnterior }),
            JsonSerializer.Serialize(new { Status = StatusPedido.Cancelado.ToString() }));

        return ToDto(pedido);
    }

    private static PedidoResponseDto ToDto(Pedido p) => new(
        p.Id,
        p.ClienteId,
        p.Cliente?.Nome ?? string.Empty,
        p.Status.ToString(),
        p.DataPedido,
        p.DataAtualizacao,
        p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
        p.Itens.Select(i => new PedidoItemResponseDto(
            i.ProdutoId,
            i.Produto?.Nome ?? string.Empty,
            i.Quantidade,
            i.PrecoUnitario,
            i.PrecoUnitario * i.Quantidade)).ToList());
}
