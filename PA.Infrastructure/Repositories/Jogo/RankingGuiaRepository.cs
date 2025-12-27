using Microsoft.EntityFrameworkCore;
using PA.Domain.Entities;
using PA.Domain.Interfaces;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class RankingGuiaRepository : IRankingGuiaRepository
{
    private readonly PastoralAppDbContext _context;

    public RankingGuiaRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<RankingGuia?> GetByIdAsync(Guid id)
    {
        return await _context.RankingsGuias
            .Include(r => r.Guia)
            .Include(r => r.Grupo)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<RankingGuia>> GetByGuiaIdAsync(Guid guiaId)
    {
        return await _context.RankingsGuias
            .Include(r => r.Grupo)
            .Where(r => r.GuiaId == guiaId)
            .OrderBy(r => r.Posicao)
            .ToListAsync();
    }

    public async Task<RankingGuia> AddAsync(RankingGuia ranking)
    {
        await _context.RankingsGuias.AddAsync(ranking);
        await _context.SaveChangesAsync();
        return ranking;
    }

    public async Task UpdateAsync(RankingGuia ranking)
    {
        ranking.DataAtualizacao = DateTime.UtcNow;
        _context.RankingsGuias.Update(ranking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var ranking = await GetByIdAsync(id);
        if (ranking != null)
        {
            _context.RankingsGuias.Remove(ranking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RecalcularRankingAsync(Guid guiaId)
    {
        var pontuacoes = await _context.PontuacoesProvas
            .Include(p => p.Prova)
            .Include(p => p.Grupo)
            .Where(p => p.Prova.GuiaId == guiaId)
            .GroupBy(p => p.GrupoId)
            .ToListAsync();

        var rankingsAtualizados = new List<RankingGuia>();

        foreach (var grupoPontuacoes in pontuacoes)
        {
            var grupoId = grupoPontuacoes.Key;
            var total = grupoPontuacoes.Sum(p => p.Pontuacao);
            var provasRealizadas = grupoPontuacoes.Select(p => p.ProvaId).Distinct().Count();
            var media = provasRealizadas > 0 ? total / provasRealizadas : 0;

            var rankingExistente = await _context.RankingsGuias
                .FirstOrDefaultAsync(r => r.GuiaId == guiaId && r.GrupoId == grupoId);

            if (rankingExistente != null)
            {
                rankingExistente.PontuacaoTotal = total;
                rankingExistente.ProvasRealizadas = provasRealizadas;
                rankingExistente.MediaPontuacao = media;
                rankingExistente.DataAtualizacao = DateTime.UtcNow;
                rankingsAtualizados.Add(rankingExistente);
            }
            else
            {
                var novoRanking = new RankingGuia
                {
                    GuiaId = guiaId,
                    GrupoId = grupoId,
                    PontuacaoTotal = total,
                    ProvasRealizadas = provasRealizadas,
                    MediaPontuacao = media,
                    DataAtualizacao = DateTime.UtcNow
                };
                await _context.RankingsGuias.AddAsync(novoRanking);
                rankingsAtualizados.Add(novoRanking);
            }
        }

        var ordenados = rankingsAtualizados
            .OrderByDescending(r => r.PontuacaoTotal)
            .ThenByDescending(r => r.MediaPontuacao)
            .ToList();

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].Posicao = i + 1;
        }

        await _context.SaveChangesAsync();
    }
}
