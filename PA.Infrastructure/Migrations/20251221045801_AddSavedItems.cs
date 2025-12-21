using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventosSalvos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSalvamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosSalvos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosSalvos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventosSalvos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsSalvos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSalvamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsSalvos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostsSalvos_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsSalvos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosSalvos_EventoId",
                table: "EventosSalvos",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosSalvos_UserId_DataSalvamento",
                table: "EventosSalvos",
                columns: new[] { "UserId", "DataSalvamento" });

            migrationBuilder.CreateIndex(
                name: "IX_EventosSalvos_UserId_EventoId",
                table: "EventosSalvos",
                columns: new[] { "UserId", "EventoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostsSalvos_PostId",
                table: "PostsSalvos",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsSalvos_UserId_DataSalvamento",
                table: "PostsSalvos",
                columns: new[] { "UserId", "DataSalvamento" });

            migrationBuilder.CreateIndex(
                name: "IX_PostsSalvos_UserId_PostId",
                table: "PostsSalvos",
                columns: new[] { "UserId", "PostId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosSalvos");

            migrationBuilder.DropTable(
                name: "PostsSalvos");
        }
    }
}
