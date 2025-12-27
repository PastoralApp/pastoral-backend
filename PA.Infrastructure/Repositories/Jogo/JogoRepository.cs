using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class JogoRepository : IJogoRepository
{
    private readonly PastoralAppDbContext _context;
    public JogoRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Jogo?> GetByIdAsync(Guid id)
    {
        return await _context.Jogos.FindAsync(id);
    }

    public async Task<Jogo?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Jogos
            .Include(j => j.Medalhas)
            .Include(j => j.Trofeus)
            .Include(j => j.Pontuacoes)
            .Include(j => j.Historico)
            .Include(j => j.Horarios)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<Jogo>> GetAllAsync()
    {
        return await _context.Jogos.ToListAsync();
    }

    public async Task<Jogo> AddAsync(Jogo jogo)
    {
        _context.Jogos.Add(jogo);
        await _context.SaveChangesAsync();
        return jogo;
    }

    public async Task<Jogo> UpdateAsync(Jogo jogo)
    {
        _context.Jogos.Update(jogo);
        await _context.SaveChangesAsync();
        return jogo;
    }

    public async Task DeleteAsync(Guid id)
    {
        var jogo = await _context.Jogos.FindAsync(id);
        if (jogo != null)
        {
            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();
        }
    }
}
