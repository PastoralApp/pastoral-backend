using Microsoft.EntityFrameworkCore;
using PA.Domain.Entities;
using PA.Domain.ValueObjects;

namespace PA.Infrastructure.Data.Context;

/// <summary>
/// DbContext principal da aplicação
/// </summary>
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
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

        // UserGrupo configuration (Many-to-Many)
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

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired();
        });

        // Pastoral configuration
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

        // Grupo configuration
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

        // Post configuration
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Type).IsRequired();

            entity.HasOne(e => e.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsPinned);
        });

        // Evento configuration
        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.EventosCreated)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.EventDate);
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(7);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Igreja configuration
        modelBuilder.Entity<Igreja>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Endereco).HasMaxLength(500);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.ImagemUrl).HasMaxLength(500);
        });

        // HorarioMissa configuration
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
    }
}
