using Microsoft.EntityFrameworkCore;
using PA.Domain.Entities;
using PA.Domain.Interfaces;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class TrofeuRepository : ITrofeuRepository
{
    private readonly PastoralAppDbContext _context;

    public TrofeuRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Trofeu?> GetByIdAsync(Guid id)
    {
        return await _context.Trofeus
            .Include(t => t.Jogo)
            .Include(t => t.Grupo)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Trofeu>> GetAllAsync()
    {
        return await _context.Trofeus
            .Include(t => t.Jogo)
            .Include(t => t.Grupo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trofeu>> GetByJogoIdAsync(Guid jogoId)
    {
        return await _context.Trofeus
            .Include(t => t.Grupo)
            .Where(t => t.JogoId == jogoId)
            .OrderBy(t => t.Posicao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trofeu>> GetByGrupoIdAsync(Guid grupoId)
    {
        return await _context.Trofeus
            .Include(t => t.Jogo)
            .Where(t => t.GrupoId == grupoId)
            .OrderByDescending(t => t.Ano)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trofeu>> GetByAnoAsync(int ano)
    {
        return await _context.Trofeus
            .Include(t => t.Jogo)
            .Include(t => t.Grupo)
            .Where(t => t.Ano == ano)
            .ToListAsync();
    }

    public async Task<Trofeu> AddAsync(Trofeu trofeu)
    {
        await _context.Trofeus.AddAsync(trofeu);
        await _context.SaveChangesAsync();
        return trofeu;
    }

    public async Task UpdateAsync(Trofeu trofeu)
    {
        _context.Trofeus.Update(trofeu);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var trofeu = await GetByIdAsync(id);
        if (trofeu != null)
        {
            _context.Trofeus.Remove(trofeu);
            await _context.SaveChangesAsync();
        }
    }
}
