using ECommerce.API.Data;
using ECommerce.API.DTOs;
using ECommerce.API.Interfaces;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services;

public class HistoricoService : IHistoricoService
{
    private readonly AppDbContext _context;

    public HistoricoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(
        TipoEntidade entidade,
        int entidadeId,
        TipoOperacao operacao,
        string dadosAnteriores,
        string dadosNovos,
        string realizadoPor = "Sistema")
    {
        var historico = new Historico
        {
            Entidade = entidade,
            EntidadeId = entidadeId,
            Operacao = operacao,
            DadosAnteriores = dadosAnteriores,
            DadosNovos = dadosNovos,
            RealizadoEm = DateTime.UtcNow,
            RealizadoPor = realizadoPor
        };

        _context.Historicos.Add(historico);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<HistoricoResponseDto>> ListarPorEntidadeAsync(TipoEntidade entidade, int entidadeId)
    {
        return await _context.Historicos
            .Where(h => h.Entidade == entidade && h.EntidadeId == entidadeId)
            .OrderByDescending(h => h.RealizadoEm)
            .Select(h => new HistoricoResponseDto(
                h.Id,
                h.Entidade.ToString(),
                h.EntidadeId,
                h.Operacao.ToString(),
                h.DadosAnteriores,
                h.DadosNovos,
                h.RealizadoEm,
                h.RealizadoPor))
            .ToListAsync();
    }
}
