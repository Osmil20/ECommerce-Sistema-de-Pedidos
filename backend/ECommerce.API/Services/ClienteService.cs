using System.Text.Json;
using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;
    private readonly IHistoricoService _historico;

    public ClienteService(AppDbContext context, IHistoricoService historico)
    {
        _context = context;
        _historico = historico;
    }

    public async Task<IEnumerable<ClienteResponseDto>> ListarAsync()
    {
        return await _context.Clientes
            .OrderBy(c => c.Nome)
            .Select(c => ToDto(c))
            .ToListAsync();
    }

    public async Task<ClienteResponseDto?> ObterPorIdAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        return cliente is null ? null : ToDto(cliente);
    }

    public async Task<ClienteResponseDto> CriarAsync(ClienteCreateDto dto)
    {
        var cliente = new Cliente
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            Email = dto.Email,
            CriadoEm = DateTime.UtcNow
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Cliente,
            cliente.Id,
            TipoOperacao.Criacao,
            string.Empty,
            JsonSerializer.Serialize(ToDto(cliente)));

        return ToDto(cliente);
    }

    public async Task<ClienteResponseDto?> AtualizarAsync(int id, ClienteUpdateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return null;

        var dadosAnteriores = JsonSerializer.Serialize(ToDto(cliente));

        cliente.Nome = dto.Nome;
        cliente.Email = dto.Email;

        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Cliente,
            cliente.Id,
            TipoOperacao.Atualizacao,
            dadosAnteriores,
            JsonSerializer.Serialize(ToDto(cliente)));

        return ToDto(cliente);
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ClienteResponseDto ToDto(Cliente c) =>
        new(c.Id, c.Nome, c.Cpf, c.Email, c.CriadoEm);
}
