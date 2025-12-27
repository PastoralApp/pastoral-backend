using Microsoft.EntityFrameworkCore;
using PA.Domain.Entities;
using PA.Domain.Interfaces;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class RankingOlimpiadasRepository : IRankingOlimpiadasRepository
{
    private readonly PastoralAppDbContext _context;

    public RankingOlimpiadasRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<RankingOlimpiadas?> GetByIdAsync(Guid id)
    {
        return await _context.RankingsOlimpiadas
            .Include(r => r.Olimpiadas)
            .Include(r => r.Grupo)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<RankingOlimpiadas>> GetByOlimpiadasIdAsync(Guid olimpiadasId)
    {
        return await _context.RankingsOlimpiadas
            .Include(r => r.Grupo)
            .Where(r => r.OlimpiadasId == olimpiadasId)
            .OrderBy(r => r.Posicao)
            .ToListAsync();
    }

    public async Task<RankingOlimpiadas> AddAsync(RankingOlimpiadas ranking)
    {
        await _context.RankingsOlimpiadas.AddAsync(ranking);
        await _context.SaveChangesAsync();
        return ranking;
    }

    public async Task UpdateAsync(RankingOlimpiadas ranking)
    {
        ranking.DataAtualizacao = DateTime.UtcNow;
        _context.RankingsOlimpiadas.Update(ranking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var ranking = await GetByIdAsync(id);
        if (ranking != null)
        {
            _context.RankingsOlimpiadas.Remove(ranking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RecalcularRankingAsync(Guid olimpiadasId)
    {
        var medalhas = await _context.Medalhas
            .Where(m => m.JogoId == olimpiadasId)
            .Include(m => m.Grupo)
            .GroupBy(m => m.GrupoId)
            .ToListAsync();

        var olimpiadas = await _context.Olimpiadas.FindAsync(olimpiadasId);
        if (olimpiadas == null) return;

        var rankingsAtualizados = new List<RankingOlimpiadas>();

        foreach (var grupoMedalhas in medalhas)
        {
            var grupoId = grupoMedalhas.Key;
            var ouro = grupoMedalhas.Count(m => m.Tipo == Domain.Enums.TipoMedalha.Ouro);
            var prata = grupoMedalhas.Count(m => m.Tipo == Domain.Enums.TipoMedalha.Prata);
            var bronze = grupoMedalhas.Count(m => m.Tipo == Domain.Enums.TipoMedalha.Bronze);

            var pontuacaoTotal = (ouro * olimpiadas.PontosOuro) + 
                                (prata * olimpiadas.PontosPrata) + 
                                (bronze * olimpiadas.PontosBronze);

            var rankingExistente = await _context.RankingsOlimpiadas
                .FirstOrDefaultAsync(r => r.OlimpiadasId == olimpiadasId && r.GrupoId == grupoId);

            if (rankingExistente != null)
            {
                rankingExistente.PontuacaoTotal = pontuacaoTotal;
                rankingExistente.MedalhasOuro = ouro;
                rankingExistente.MedalhasPrata = prata;
                rankingExistente.MedalhasBronze = bronze;
                rankingExistente.DataAtualizacao = DateTime.UtcNow;
                rankingsAtualizados.Add(rankingExistente);
            }
            else
            {
                var novoRanking = new RankingOlimpiadas
                {
                    OlimpiadasId = olimpiadasId,
                    GrupoId = grupoId,
                    PontuacaoTotal = pontuacaoTotal,
                    MedalhasOuro = ouro,
                    MedalhasPrata = prata,
                    MedalhasBronze = bronze,
                    DataAtualizacao = DateTime.UtcNow
                };
                await _context.RankingsOlimpiadas.AddAsync(novoRanking);
                rankingsAtualizados.Add(novoRanking);
            }
        }

        var ordenados = rankingsAtualizados
            .OrderByDescending(r => r.PontuacaoTotal)
            .ThenByDescending(r => r.MedalhasOuro)
            .ThenByDescending(r => r.MedalhasPrata)
            .ThenByDescending(r => r.MedalhasBronze)
            .ToList();

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].Posicao = i + 1;
        }

        await _context.SaveChangesAsync();
    }
}
