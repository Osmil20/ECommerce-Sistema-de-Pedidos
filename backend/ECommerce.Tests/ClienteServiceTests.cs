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

public class ClienteServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var historicoMock = new Mock<IHistoricoService>();
        _service = new ClienteService(_context, historicoMock.Object);
    }

    [Fact(DisplayName = "Deve criar cliente com todos os campos preenchidos")]
    public async Task CriarAsync_DevePersistirCliente_ComDadosCorretos()
    {
        var dto = new ClienteCreateDto("Ana Paula", "000.111.222-33", "ana@email.com");

        var resultado = await _service.CriarAsync(dto);

        resultado.Id.Should().BeGreaterThan(0);
        resultado.Nome.Should().Be("Ana Paula");
        resultado.Cpf.Should().Be("000.111.222-33");
        resultado.Email.Should().Be("ana@email.com");
    }

    [Fact(DisplayName = "Deve atualizar cliente e preservar CPF original")]
    public async Task AtualizarAsync_DeveAtualizarNomeEEmail_SemAlterarCpf()
    {
        var criado = await _service.CriarAsync(new ClienteCreateDto("Nome Antigo", "123.456.789-00", "antigo@email.com"));

        var atualizado = await _service.AtualizarAsync(criado.Id, new ClienteUpdateDto("Nome Novo", "novo@email.com"));

        atualizado.Should().NotBeNull();
        atualizado!.Nome.Should().Be("Nome Novo");
        atualizado.Email.Should().Be("novo@email.com");
        atualizado.Cpf.Should().Be("123.456.789-00");
    }

    [Fact(DisplayName = "Deve retornar null ao tentar atualizar cliente inexistente")]
    public async Task AtualizarAsync_DeveRetornarNull_QuandoClienteNaoExiste()
    {
        var resultado = await _service.AtualizarAsync(9999, new ClienteUpdateDto("Qualquer", "q@q.com"));
        resultado.Should().BeNull();
    }

    [Fact(DisplayName = "Deve listar clientes em ordem alfabética")]
    public async Task ListarAsync_DeveRetornarClientesOrdenadosPorNome()
    {
        await _service.CriarAsync(new ClienteCreateDto("Zacarias", "111.111.111-11", "z@z.com"));
        await _service.CriarAsync(new ClienteCreateDto("Ana", "222.222.222-22", "a@a.com"));

        var lista = (await _service.ListarAsync()).ToList();

        lista.First().Nome.Should().Be("Ana");
        lista.Last().Nome.Should().Be("Zacarias");
    }

    [Fact(DisplayName = "Deve remover cliente existente")]
    public async Task RemoverAsync_DeveRetornarTrue_QuandoClienteExiste()
    {
        var criado = await _service.CriarAsync(new ClienteCreateDto("Para Remover", "999.888.777-66", "rm@rm.com"));

        var resultado = await _service.RemoverAsync(criado.Id);

        resultado.Should().BeTrue();
        (await _service.ObterPorIdAsync(criado.Id)).Should().BeNull();
    }

    public void Dispose() => _context.Dispose();
}
