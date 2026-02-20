using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using ECommerce.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ECommerce.Tests;

public class PedidoServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IHistoricoService> _historicoMock;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _historicoMock = new Mock<IHistoricoService>();
        _service = new PedidoService(_context, _historicoMock.Object);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.Clientes.Add(new Cliente { Id = 10, Nome = "Cliente Teste", Cpf = "111.111.111-11", Email = "teste@teste.com" });
        _context.Produtos.Add(new Produto { Id = 20, Nome = "Produto A", Preco = 100m, Estoque = 5 });
        _context.Produtos.Add(new Produto { Id = 21, Nome = "Produto B", Preco = 50m, Estoque = 0 });
        _context.SaveChanges();
    }

    [Fact(DisplayName = "Deve criar pedido com sucesso quando cliente e produtos são válidos")]
    public async Task CriarAsync_DeveRetornarPedido_QuandoDadosValidos()
    {
        var dto = new PedidoCreateDto(10, new List<ItemPedidoDto> { new(20, 2) });

        var resultado = await _service.CriarAsync(dto);

        resultado.Should().NotBeNull();
        resultado.ClienteId.Should().Be(10);
        resultado.Status.Should().Be("Criado");
        resultado.ValorTotal.Should().Be(200m);
        resultado.Itens.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando produto não tem estoque suficiente")]
    public async Task CriarAsync_DeveLancarExcecao_QuandoEstoqueInsuficiente()
    {
        var dto = new PedidoCreateDto(10, new List<ItemPedidoDto> { new(21, 1) });

        var acao = async () => await _service.CriarAsync(dto);

        await acao.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Estoque insuficiente*");
    }

    [Fact(DisplayName = "Deve lançar exceção quando lista de itens está vazia")]
    public async Task CriarAsync_DeveLancarExcecao_QuandoSemItens()
    {
        var dto = new PedidoCreateDto(10, new List<ItemPedidoDto>());

        var acao = async () => await _service.CriarAsync(dto);

        await acao.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ao menos um produto*");
    }

    [Fact(DisplayName = "Deve pagar pedido e alterar status para Pago")]
    public async Task PagarAsync_DeveAlterarStatusParaPago_QuandoPedidoCriado()
    {
        var pedido = CriarPedidoNoBanco();

        var resultado = await _service.PagarAsync(pedido.Id);

        resultado.Should().NotBeNull();
        resultado!.Status.Should().Be("Pago");
    }

    [Fact(DisplayName = "Deve lançar exceção ao tentar pagar pedido já pago")]
    public async Task PagarAsync_DeveLancarExcecao_QuandoPedidoJaPago()
    {
        var pedido = CriarPedidoNoBanco(StatusPedido.Pago);

        var acao = async () => await _service.PagarAsync(pedido.Id);

        await acao.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve cancelar pedido e restaurar estoque")]
    public async Task CancelarAsync_DeveAlterarStatusERestaurarEstoque_QuandoPedidoCriado()
    {
        var estoqueAntes = _context.Produtos.Find(20)!.Estoque;
        var pedido = CriarPedidoNoBanco();

        var resultado = await _service.CancelarAsync(pedido.Id);

        resultado.Should().NotBeNull();
        resultado!.Status.Should().Be("Cancelado");

        var estoqueDepois = _context.Produtos.Find(20)!.Estoque;
        estoqueDepois.Should().Be(estoqueAntes + 2);
    }

    [Fact(DisplayName = "Deve lançar exceção ao tentar cancelar pedido já pago")]
    public async Task CancelarAsync_DeveLancarExcecao_QuandoPedidoJaPago()
    {
        var pedido = CriarPedidoNoBanco(StatusPedido.Pago);

        var acao = async () => await _service.CancelarAsync(pedido.Id);

        await acao.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve listar pedidos filtrados por status")]
    public async Task ListarAsync_DeveRetornarApenasPedidosDoStatus_QuandoFiltroInformado()
    {
        CriarPedidoNoBanco(StatusPedido.Pago);
        CriarPedidoNoBanco(StatusPedido.Criado);
        CriarPedidoNoBanco(StatusPedido.Cancelado);

        var pagos = await _service.ListarAsync(StatusPedido.Pago);

        pagos.Should().OnlyContain(p => p.Status == "Pago");
    }

    [Fact(DisplayName = "Deve retornar null quando pedido não existe")]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoPedidoNaoExiste()
    {
        var resultado = await _service.ObterPorIdAsync(9999);
        resultado.Should().BeNull();
    }

    private Pedido CriarPedidoNoBanco(StatusPedido status = StatusPedido.Criado)
    {
        var produto = _context.Produtos.Find(20)!;
        var pedido = new Pedido
        {
            ClienteId = 10,
            Status = status,
            DataPedido = DateTime.UtcNow,
            Itens = new List<PedidoProduto>
            {
                new() { ProdutoId = 20, Produto = produto, Quantidade = 2, PrecoUnitario = 100m }
            }
        };

        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
        return pedido;
    }

    public void Dispose() => _context.Dispose();
}
