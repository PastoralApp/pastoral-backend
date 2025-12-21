using Microsoft.EntityFrameworkCore;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Infrastructure.Data.Context;
using System.Linq.Expressions;

namespace PA.Infrastructure.Repositories;

public class NotificacaoRepository : INotificacaoRepository
{
    protected readonly PastoralAppDbContext _context;

    public NotificacaoRepository(PastoralAppDbContext context)
    {
        _context = context;
    }

    public async Task<Notificacao?> GetByIdAsync(Guid id)
    {
        return await _context.Notificacoes
            .Include(n => n.Grupo)
            .Include(n => n.Remetente)
            .Include(n => n.Leituras)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Notificacao>> GetAllAsync()
    {
        return await _context.Notificacoes
            .Include(n => n.Grupo)
            .Include(n => n.Remetente)
            .Include(n => n.Leituras)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacao>> FindAsync(Expression<Func<Notificacao, bool>> predicate)
    {
        return await _context.Notificacoes
            .Include(n => n.Grupo)
            .Include(n => n.Remetente)
            .Include(n => n.Leituras)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Notificacao> AddAsync(Notificacao entity)
    {
        await _context.Notificacoes.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Notificacao entity)
    {
        _context.Notificacoes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Notificacoes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Notificacoes.AnyAsync(n => n.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Notificacoes.CountAsync();
    }

    public async Task<IEnumerable<Notificacao>> GetByGrupoIdAsync(Guid grupoId, bool incluirInativas = false)
    {
        var query = _context.Notificacoes
            .Include(n => n.Grupo)
            .Include(n => n.Remetente)
            .Include(n => n.Leituras)
            .Where(n => n.GrupoId == grupoId);

        if (!incluirInativas)
            query = query.Where(n => n.IsAtiva);

        return await query
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacao>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notificacoes
            .Include(n => n.Grupo)
            .Include(n => n.Remetente)
            .Include(n => n.Leituras)
            .Where(n => n.IsAtiva && 
                (n.IsGeral || 
                 (n.GrupoId != null && n.Grupo!.UserGrupos.Any(ug => ug.UserId == userId && ug.IsAtivo && !ug.SilenciarNotificacoes))))
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacao>> GetNaoLidasByUserIdAsync(Guid userId)
    {
        return await _context.Notificacoes
            .Include(n => n.Grupo)
            .Include(n => n.Remetente)
            .Include(n => n.Leituras)
            .Where(n => n.IsAtiva 
                && (n.IsGeral || (n.GrupoId != null && n.Grupo!.UserGrupos.Any(ug => ug.UserId == userId && ug.IsAtivo && !ug.SilenciarNotificacoes)))
                && !n.Leituras.Any(l => l.UserId == userId))
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<NotificacaoLeitura?> GetLeituraAsync(Guid notificacaoId, Guid userId)
    {
        return await _context.Set<NotificacaoLeitura>()
            .FirstOrDefaultAsync(l => l.NotificacaoId == notificacaoId && l.UserId == userId);
    }

    public async Task MarcarComoLidaAsync(Guid notificacaoId, Guid userId)
    {
        var leituraExistente = await GetLeituraAsync(notificacaoId, userId);
        if (leituraExistente == null)
        {
            var leitura = new NotificacaoLeitura(notificacaoId, userId);
            await _context.Set<NotificacaoLeitura>().AddAsync(leitura);
            await _context.SaveChangesAsync();
        }
    }
}
