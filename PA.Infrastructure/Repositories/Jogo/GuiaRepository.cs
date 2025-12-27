using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Repositories;

public class GuiaRepository : IGuiaRepository
{
    private readonly PastoralAppDbContext _context;
    public GuiaRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guia?> GetByIdAsync(Guid id)
    {
        return await _context.Guias.FindAsync(id);
    }

    public async Task<Guia?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Guias
            .Include(g => g.Provas)
            .Include(g => g.PontuacoesProvas)
            .Include(g => g.Rankings)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Guia>> GetAllAsync()
    {
        return await _context.Guias.ToListAsync();
    }

    public async Task<Guia> AddAsync(Guia guia)
    {
        _context.Guias.Add(guia);
        await _context.SaveChangesAsync();
        return guia;
    }

    public async Task<Guia> UpdateAsync(Guia guia)
    {
        _context.Guias.Update(guia);
        await _context.SaveChangesAsync();
        return guia;
    }

    public async Task DeleteAsync(Guid id)
    {
        var guia = await _context.Guias.FindAsync(id);
        if (guia != null)
        {
            _context.Guias.Remove(guia);
            await _context.SaveChangesAsync();
        }
    }
}
