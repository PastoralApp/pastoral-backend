using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PastoralAppDbContext context)
    {
        if (context.Roles.Any())
            return;

        Console.WriteLine("🌱 Iniciando seed do banco de dados...");

        var roles = new[]
        {
            new Role(
                name: "Usuário",
                type: RoleType.Usuario,
                description: "Usuário comum da plataforma"
            ),
            new Role(
                name: "Coordenador de Grupo",
                type: RoleType.CoordenadorGrupo,
                description: "Coordenador de um grupo pastoral"
            ),
            new Role(
                name: "Coordenador Geral",
                type: RoleType.CoordenadorGeral,
                description: "Coordenador geral da pastoral"
            ),
            new Role(
                name: "Administrador",
                type: RoleType.Admin,
                description: "Administrador do sistema"
            )
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Roles criadas");

        var paProfetaElias = new Pastoral(
            name: "Pastoral Adolescente Profeta Elias",
            sigla: "PA-PPE",
            tipoPastoral: TipoPastoral.PA,
            type: PastoralType.PA,
            theme: new ColorTheme("#8B4789", "#E0BBE4"),
            description: "Pastoral Adolescente da Paróquia Profeta Elias"
        );

        var paPansa = new Pastoral(
            name: "Pastoral Adolescente Nossa Senhora Aparecida",
            sigla: "PA-PANSA",
            tipoPastoral: TipoPastoral.PA,
            type: PastoralType.PA,
            theme: new ColorTheme("#1E40AF", "#DBEAFE"),
            description: "Pastoral Adolescente da Paróquia Nossa Senhora Aparecida"
        );

        var pjcProfetaElias = new Pastoral(
            name: "Pastoral Juventude Carmelitana",
            sigla: "PJC",
            tipoPastoral: TipoPastoral.PJ,
            type: PastoralType.PJ,
            theme: new ColorTheme("#78350F", "#FEF3C7"),
            description: "Pastoral da Juventude Carmelitana - Paróquia Profeta Elias"
        );

        var pjaPansa = new Pastoral(
            name: "Pastoral Juventude Aparecida",
            sigla: "PJA",
            tipoPastoral: TipoPastoral.PJ,
            type: PastoralType.PJ,
            theme: new ColorTheme("#DC2626", "#FEE2E2"),
            description: "Pastoral da Juventude Aparecida - Paróquia N.S Aparecida"
        );

        await context.Pastorais.AddRangeAsync(paProfetaElias, paPansa, pjcProfetaElias, pjaPansa);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Pastorais criadas");

        var gruposPaProfetaElias = new[]
        {
            new Grupo("Adolescentes Renascendo na Fé", "AREF", "Grupo de adolescentes focado em renovação espiritual", paProfetaElias.Id, new ColorTheme("#DC2626", "#FEE2E2")),
            new Grupo("Adolescentes Unidos em Cristo", "AUC", "União de adolescentes na fé católica", paProfetaElias.Id, new ColorTheme("#2563EB", "#DBEAFE")),
            new Grupo("Adolescentes Gerando Amor Pelo Espírito Santo", "AGAPES", "Amor e comunhão no Espírito Santo", paProfetaElias.Id, new ColorTheme("#7C3AED", "#EDE9FE")),
            new Grupo("Adolescentes Preservando a Vida", "APAV", "Promoção da cultura da vida", paProfetaElias.Id, new ColorTheme("#059669", "#D1FAE5")),
            new Grupo("Adolescentes Juntos no Amor de Deus", "AJAD", "Unidos no amor de Deus", paProfetaElias.Id, new ColorTheme("#EA580C", "#FED7AA")),
            new Grupo("Adolescentes Unidos Confirmando o Amor de Cristo", "AUCAC", "Confirmação do amor de Cristo", paProfetaElias.Id, new ColorTheme("#DB2777", "#FCE7F3"))
        };

        var gruposPaPansa = new[]
        {
            new Grupo("Adolescentes Unidos Pelo Espírito Santo", "AUPES", "Unidos no Espírito Santo", paPansa.Id, new ColorTheme("#0891B2", "#CFFAFE")),
            new Grupo("Adolescentes Preservando a Vida", "APAV", "Cultura da vida", paPansa.Id, new ColorTheme("#059669", "#D1FAE5")),
            new Grupo("Jovens Adolescentes no Amor de Deus", "JAAV", "Amor divino entre jovens", paPansa.Id, new ColorTheme("#9333EA", "#F3E8FF")),
            new Grupo("Adolescentes Lutando pela Fé Ardente e Salvação", "ALFAS", "Fé ardente e salvação", paPansa.Id, new ColorTheme("#DC2626", "#FEE2E2")),
            new Grupo("Adolescentes Unidos em Cristo", "AUC", "Unidade em Cristo", paPansa.Id, new ColorTheme("#2563EB", "#DBEAFE")),
            new Grupo("Adolescentes Caminhando Alegremente com Luz de Jesus", "ACALJ", "Caminhando com alegria na luz de Jesus", paPansa.Id, new ColorTheme("#F59E0B", "#FEF3C7"))
        };

        var gruposPjc = new[]
        {
            new Grupo("Encontristas de Jesus Amor e Caridade", "EJAC", "Encontristas unidos no amor de Cristo", pjcProfetaElias.Id, new ColorTheme("#78350F", "#FEF3C7")),
            new Grupo("Jovens Unidos Renascendo na Fé", "JUREF", "Renovação espiritual dos jovens", pjcProfetaElias.Id, new ColorTheme("#92400E", "#FED7AA")),
            new Grupo("Jovens Abraçando Deus Eternamente", "JADE", "Jovens em comunhão eterna com Deus", pjcProfetaElias.Id, new ColorTheme("#A16207", "#FEF3C7")),
            new Grupo("Jovens Unidos Caminhando Alegremente no Paraíso", "JUCAP", "Caminhada alegre rumo ao céu", pjcProfetaElias.Id, new ColorTheme("#854D0E", "#FFFBEB"))
        };

        var gruposPja = new[]
        {
            new Grupo("Jovens Amando Cristo Onipotente", "JACO", "Amor a Cristo onipotente", pjaPansa.Id, new ColorTheme("#7F1D1D", "#FEE2E2")),
            new Grupo("Jovens Unidos Caminhando com Cristo", "JUCC", "Caminhada jovem com Cristo", pjaPansa.Id, new ColorTheme("#991B1B", "#FECACA")),
            new Grupo("Jovens Unidos pelo Espírito Santo", "JUPES", "Unidos no Espírito Santo", pjaPansa.Id, new ColorTheme("#B91C1C", "#FCA5A5")),
            new Grupo("Jovens Unidos Seguindo Iluminados", "JUSI", "Seguindo a luz de Cristo", pjaPansa.Id, new ColorTheme("#DC2626", "#FEE2E2")),
            new Grupo("Jovens Unidos Seguindo Alegremente Cristo", "JUSAC", "Seguindo Cristo com alegria", pjaPansa.Id, new ColorTheme("#EF4444", "#FEE2E2"))
        };

        await context.Grupos.AddRangeAsync(gruposPaProfetaElias);
        await context.Grupos.AddRangeAsync(gruposPaPansa);
        await context.Grupos.AddRangeAsync(gruposPjc);
        await context.Grupos.AddRangeAsync(gruposPja);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Grupos criados");

        var igrejaMatriz = new Igreja(
            nome: "Igreja Matriz",
            endereco: "Endereço da Igreja Matriz",
            telefone: "(11) 1234-5678"
        );

        await context.Igrejas.AddAsync(igrejaMatriz);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Igrejas criadas");

        var horariosMissa = new[]
        {
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Sunday, new TimeSpan(8, 0, 0), "Pe. FREI VAGNER"),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Sunday, new TimeSpan(10, 30, 0), "Pe. FREI VAGNER"),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Sunday, new TimeSpan(19, 30, 0), "Pe. FREI VAGNER"),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Saturday, new TimeSpan(19, 30, 0), "Pe. FREI VAGNER"),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Monday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Tuesday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Wednesday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Thursday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Friday, new TimeSpan(19, 0, 0))
        };

        await context.HorariosMissas.AddRangeAsync(horariosMissa);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Horários de Missa criados");

        var adminRole = roles.First(r => r.Type == RoleType.Admin);

        var adminEmail = new Email("admin@admin.com");
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@admin");

        var adminUser = new User(
            name: "Administrador",
            email: adminEmail,
            passwordHash: adminPasswordHash,
            roleId: adminRole.Id
        );

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Usuário admin criado");
        Console.WriteLine($"   Email: admin@admin.com");
        Console.WriteLine($"   Senha: admin@admin");
        Console.WriteLine("🎉 Seed concluído!");
    }
}
