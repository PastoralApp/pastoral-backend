using Microsoft.EntityFrameworkCore;
using PA.Domain.Entities;
using PA.Domain.ValueObjects;

namespace PA.Infrastructure.Data.Context;

public class PastoralAppDbContext : DbContext
{
    public PastoralAppDbContext(DbContextOptions<PastoralAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Pastoral> Pastorais => Set<Pastoral>();
    public DbSet<Grupo> Grupos => Set<Grupo>();
    public DbSet<UserGrupo> UserGrupos => Set<UserGrupo>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Igreja> Igrejas => Set<Igreja>();
    public DbSet<HorarioMissa> HorariosMissas => Set<HorarioMissa>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<PostReaction> PostReactions => Set<PostReaction>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostShare> PostShares => Set<PostShare>();
    public DbSet<PostSalvo> PostsSalvos => Set<PostSalvo>();
    public DbSet<EventoSalvo> EventosSalvos => Set<EventoSalvo>();
    public DbSet<EventoParticipante> EventoParticipantes => Set<EventoParticipante>();
    public DbSet<EmailVerification> EmailVerifications => Set<EmailVerification>();
    public DbSet<Jogo> Jogos => Set<Jogo>();
    public DbSet<Olimpiadas> Olimpiadas => Set<Olimpiadas>();
    public DbSet<Guia> Guias => Set<Guia>();
    public DbSet<Modalidade> Modalidades => Set<Modalidade>();
    public DbSet<Prova> Provas => Set<Prova>();
    public DbSet<Medalha> Medalhas => Set<Medalha>();
    public DbSet<Trofeu> Trofeus => Set<Trofeu>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<GrupoJogo> GruposJogos => Set<GrupoJogo>();
    public DbSet<GrupoModalidade> GruposModalidades => Set<GrupoModalidade>();
    public DbSet<ParticipanteGrupo> ParticipantesGrupos => Set<ParticipanteGrupo>();
    public DbSet<ParticipanteModalidade> ParticipantesModalidades => Set<ParticipanteModalidade>();
    public DbSet<MedalhaParticipante> MedalhasParticipantes => Set<MedalhaParticipante>();
    public DbSet<PontuacaoJogo> PontuacoesJogos => Set<PontuacaoJogo>();
    public DbSet<PontuacaoProva> PontuacoesProvas => Set<PontuacaoProva>();
    public DbSet<HorarioJogo> HorariosJogos => Set<HorarioJogo>();
    public DbSet<HistoricoJogo> HistoricosJogos => Set<HistoricoJogo>();
    public DbSet<ChaveOlimpiadas> ChavesOlimpiadas => Set<ChaveOlimpiadas>();
    public DbSet<ChaveModalidade> ChavesModalidades => Set<ChaveModalidade>();
    public DbSet<GrupoChave> GruposChaves => Set<GrupoChave>();
    public DbSet<RankingOlimpiadas> RankingsOlimpiadas => Set<RankingOlimpiadas>();
    public DbSet<RankingGuia> RankingsGuias => Set<RankingGuia>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            
            entity.OwnsOne(e => e.Email, email =>
            {
                email.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("Email");
            });

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Tags)
                .WithMany(t => t.Users)
                .UsingEntity(j => j.ToTable("UserTags"));
        });

        modelBuilder.Entity<UserGrupo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserGrupos)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Grupo)
                .WithMany(g => g.UserGrupos)
                .HasForeignKey(e => e.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.GrupoId }).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired();
        });

        modelBuilder.Entity<Pastoral>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Sigla).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.TipoPastoral).IsRequired();
            entity.Property(e => e.LogoUrl).HasMaxLength(500);

            entity.OwnsOne(e => e.Theme, theme =>
            {
                theme.Property(t => t.PrimaryColor)
                    .IsRequired()
                    .HasMaxLength(7)
                    .HasColumnName("PrimaryColor");
                
                theme.Property(t => t.SecondaryColor)
                    .IsRequired()
                    .HasMaxLength(7)
                    .HasColumnName("SecondaryColor");
            });
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Sigla).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);

            entity.HasOne(e => e.Pastoral)
                .WithMany(p => p.Grupos)
                .HasForeignKey(e => e.PastoralId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Igreja)
                .WithMany()
                .HasForeignKey(e => e.IgrejaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.OwnsOne(e => e.Theme, theme =>
            {
                theme.Property(t => t.PrimaryColor)
                    .IsRequired()
                    .HasMaxLength(7)
                    .HasColumnName("PrimaryColor");
                
                theme.Property(t => t.SecondaryColor)
                    .IsRequired()
                    .HasMaxLength(7)
                    .HasColumnName("SecondaryColor");
            });
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.PinType).HasMaxLength(50);

            entity.HasOne(e => e.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsPinned);
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.LinkInscricao).HasMaxLength(500);
            entity.Property(e => e.Preco).HasPrecision(10, 2);

            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.EventosCreated)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Participantes)
                .WithOne(p => p.Evento)
                .HasForeignKey(p => p.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.EventDate);
            entity.HasIndex(e => e.Type);
        });

        modelBuilder.Entity<EventoParticipante>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Evento)
                .WithMany(ev => ev.Participantes)
                .HasForeignKey(e => e.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.EventoId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(7);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Igreja>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Endereco).HasMaxLength(500);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.ImagemUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<HorarioMissa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiaSemana).IsRequired();
            entity.Property(e => e.Horario).IsRequired();
            entity.Property(e => e.Celebrante).HasMaxLength(200);
            entity.Property(e => e.Observacao).HasMaxLength(500);

            entity.HasOne(e => e.Igreja)
                .WithMany(i => i.HorariosMissas)
                .HasForeignKey(e => e.IgrejaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.IgrejaId, e.DiaSemana, e.Horario });
        });

        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.DataEnvio).IsRequired();
            entity.Property(e => e.IsGeral).IsRequired();
            entity.Property(e => e.SendEmail).IsRequired();

            entity.HasOne(e => e.Grupo)
                .WithMany()
                .HasForeignKey(e => e.GrupoId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            entity.HasOne(e => e.Destinatario)
                .WithMany()
                .HasForeignKey(e => e.DestinatarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            entity.HasOne(e => e.Remetente)
                .WithMany()
                .HasForeignKey(e => e.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.GrupoId, e.DataEnvio });
            entity.HasIndex(e => e.DestinatarioId);
        });

        modelBuilder.Entity<NotificacaoLeitura>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Notificacao)
                .WithMany(n => n.Leituras)
                .HasForeignKey(e => e.NotificacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.NotificacaoId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<PostReaction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.PostId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Conteudo).IsRequired().HasMaxLength(1000);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.PostId, e.DataComentario });
        });

        modelBuilder.Entity<PostShare>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Shares)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.PostId, e.UserId });
        });

        modelBuilder.Entity<PostSalvo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.DataSalvamento });
        });

        modelBuilder.Entity<EventoSalvo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Evento)
                .WithMany()
                .HasForeignKey(e => e.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.EventoId }).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.DataSalvamento });
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired();
            entity.Property(e => e.Purpose).HasMaxLength(50);

            entity.HasIndex(e => new { e.Email, e.Code });
            entity.HasIndex(e => e.ExpiresAt);
        });
    }
}
