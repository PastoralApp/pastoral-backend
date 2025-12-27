using Microsoft.EntityFrameworkCore;
using PA.Domain.Entities;
using PA.Domain.Interfaces;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class MedalhaRepository : IMedalhaRepository
{
    private readonly PastoralAppDbContext _context;

    public MedalhaRepository(PastoralAppDbContext _context)
    {
        this._context = _context;
    }

    public async Task<Medalha?> GetByIdAsync(Guid id)
    {
        return await _context.Medalhas
            .Include(m => m.Jogo)
            .Include(m => m.Grupo)
            .Include(m => m.Modalidade)
            .Include(m => m.Prova)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Medalha>> GetAllAsync()
    {
        return await _context.Medalhas
            .Include(m => m.Jogo)
            .Include(m => m.Grupo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Medalha>> GetByJogoIdAsync(Guid jogoId)
    {
        return await _context.Medalhas
            .Include(m => m.Grupo)
            .Include(m => m.Modalidade)
            .Include(m => m.Prova)
            .Where(m => m.JogoId == jogoId)
            .OrderBy(m => m.Tipo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Medalha>> GetByGrupoIdAsync(Guid grupoId)
    {
        return await _context.Medalhas
            .Include(m => m.Jogo)
            .Include(m => m.Modalidade)
            .Include(m => m.Prova)
            .Where(m => m.GrupoId == grupoId)
            .OrderByDescending(m => m.Ano)
            .ThenBy(m => m.Tipo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Medalha>> GetByParticipanteIdAsync(Guid participanteId)
    {
        return await _context.Medalhas
            .Include(m => m.Jogo)
            .Include(m => m.Grupo)
            .Include(m => m.Participantes)
            .Where(m => m.Participantes.Any(p => p.UserId == participanteId))
            .OrderByDescending(m => m.Ano)
            .ToListAsync();
    }

    public async Task<IEnumerable<Medalha>> GetByAnoAsync(int ano)
    {
        return await _context.Medalhas
            .Include(m => m.Jogo)
            .Include(m => m.Grupo)
            .Where(m => m.Ano == ano)
            .ToListAsync();
    }

    public async Task<Medalha> AddAsync(Medalha medalha)
    {
        await _context.Medalhas.AddAsync(medalha);
        await _context.SaveChangesAsync();
        return medalha;
    }

    public async Task UpdateAsync(Medalha medalha)
    {
        _context.Medalhas.Update(medalha);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var medalha = await GetByIdAsync(id);
        if (medalha != null)
        {
            _context.Medalhas.Remove(medalha);
            await _context.SaveChangesAsync();
        }
    }
}
