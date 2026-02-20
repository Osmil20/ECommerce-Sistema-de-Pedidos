using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ECommerce.Tests;

public class ProdutoServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProdutoService _service;

    public ProdutoServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var historicoMock = new Mock<IHistoricoService>();
        _service = new ProdutoService(_context, historicoMock.Object);
    }

    [Fact(DisplayName = "Deve criar produto com preço e estoque corretos")]
    public async Task CriarAsync_DevePersistirProduto_ComPrecoEEstoque()
    {
        var dto = new ProdutoCreateDto("Monitor LG 27\"", 1299.90m, "Monitor IPS 4K", 15);

        var resultado = await _service.CriarAsync(dto);

        resultado.Id.Should().BeGreaterThan(0);
        resultado.Preco.Should().Be(1299.90m);
        resultado.Estoque.Should().Be(15);
    }

    [Fact(DisplayName = "Deve atualizar todos os campos do produto")]
    public async Task AtualizarAsync_DeveAtualizarPrecoEEstoque()
    {
        var criado = await _service.CriarAsync(new ProdutoCreateDto("Produto X", 10m, "Desc", 5));

        var atualizado = await _service.AtualizarAsync(criado.Id, new ProdutoUpdateDto("Produto X v2", 20m, "Desc nova", 10));

        atualizado.Should().NotBeNull();
        atualizado!.Preco.Should().Be(20m);
        atualizado.Estoque.Should().Be(10);
    }

    [Fact(DisplayName = "Deve retornar null ao tentar atualizar produto inexistente")]
    public async Task AtualizarAsync_DeveRetornarNull_QuandoProdutoNaoExiste()
    {
        var resultado = await _service.AtualizarAsync(9999, new ProdutoUpdateDto("X", 1m, "X", 1));
        resultado.Should().BeNull();
    }

    public void Dispose() => _context.Dispose();
}
