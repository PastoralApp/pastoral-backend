using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacoesESilenciar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Grupos_GrupoId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GrupoId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GrupoId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Pastorais",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sigla",
                table: "Pastorais",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TipoPastoral",
                table: "Pastorais",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Grupos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sigla",
                table: "Grupos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Igrejas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ImagemUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAtiva = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Igrejas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GrupoId = table.Column<Guid>(type: "uuid", nullable: true),
                    RemetenteId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtiva = table.Column<bool>(type: "boolean", nullable: false),
                    IsGeral = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Users_RemetenteId",
                        column: x => x.RemetenteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGrupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    SilenciarNotificacoes = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGrupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGrupos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HorariosMissas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IgrejaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    Horario = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Celebrante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosMissas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosMissas_Igrejas_IgrejaId",
                        column: x => x.IgrejaId,
                        principalTable: "Igrejas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificacaoLeitura",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacaoLeitura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacaoLeitura_Notificacoes_NotificacaoId",
                        column: x => x.NotificacaoId,
                        principalTable: "Notificacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificacaoLeitura_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HorariosMissas_IgrejaId_DiaSemana_Horario",
                table: "HorariosMissas",
                columns: new[] { "IgrejaId", "DiaSemana", "Horario" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoLeitura_NotificacaoId_UserId",
                table: "NotificacaoLeitura",
                columns: new[] { "NotificacaoId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoLeitura_UserId",
                table: "NotificacaoLeitura",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_GrupoId_DataEnvio",
                table: "Notificacoes",
                columns: new[] { "GrupoId", "DataEnvio" });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_RemetenteId",
                table: "Notificacoes",
                column: "RemetenteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrupos_GrupoId",
                table: "UserGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrupos_UserId_GrupoId",
                table: "UserGrupos",
                columns: new[] { "UserId", "GrupoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorariosMissas");

            migrationBuilder.DropTable(
                name: "NotificacaoLeitura");

            migrationBuilder.DropTable(
                name: "UserGrupos");

            migrationBuilder.DropTable(
                name: "Igrejas");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Pastorais");

            migrationBuilder.DropColumn(
                name: "Sigla",
                table: "Pastorais");

            migrationBuilder.DropColumn(
                name: "TipoPastoral",
                table: "Pastorais");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "Sigla",
                table: "Grupos");

            migrationBuilder.AddColumn<Guid>(
                name: "GrupoId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GrupoId",
                table: "Users",
                column: "GrupoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Grupos_GrupoId",
                table: "Users",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
