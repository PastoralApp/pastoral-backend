using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelefoneToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPastoral",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Pastorais",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Grupos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TipoPastoral",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Pastorais");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Grupos");
        }
    }
}
