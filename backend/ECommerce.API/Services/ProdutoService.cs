using System.Text.Json;
using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services;

public class ProdutoService : IProdutoService
{
    private readonly AppDbContext _context;
    private readonly IHistoricoService _historico;

    public ProdutoService(AppDbContext context, IHistoricoService historico)
    {
        _context = context;
        _historico = historico;
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ListarAsync()
    {
        return await _context.Produtos
            .OrderBy(p => p.Nome)
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<ProdutoResponseDto?> ObterPorIdAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        return produto is null ? null : ToDto(produto);
    }

    public async Task<ProdutoResponseDto> CriarAsync(ProdutoCreateDto dto)
    {
        var produto = new Produto
        {
            Nome = dto.Nome,
            Preco = dto.Preco,
            Descricao = dto.Descricao,
            Estoque = dto.Estoque,
            CriadoEm = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Produto,
            produto.Id,
            TipoOperacao.Criacao,
            string.Empty,
            JsonSerializer.Serialize(ToDto(produto)));

        return ToDto(produto);
    }

    public async Task<ProdutoResponseDto?> AtualizarAsync(int id, ProdutoUpdateDto dto)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return null;

        var dadosAnteriores = JsonSerializer.Serialize(ToDto(produto));

        produto.Nome = dto.Nome;
        produto.Preco = dto.Preco;
        produto.Descricao = dto.Descricao;
        produto.Estoque = dto.Estoque;

        await _context.SaveChangesAsync();

        await _historico.RegistrarAsync(
            TipoEntidade.Produto,
            produto.Id,
            TipoOperacao.Atualizacao,
            dadosAnteriores,
            JsonSerializer.Serialize(ToDto(produto)));

        return ToDto(produto);
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return false;

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ProdutoResponseDto ToDto(Produto p) =>
        new(p.Id, p.Nome, p.Preco, p.Descricao, p.Estoque);
}
