using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insania.Files.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityIdToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "entity_id",
                schema: "insania_files",
                table: "r_files",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Идентификатор сущности");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "entity_id",
                schema: "insania_files",
                table: "r_files");
        }
    }
}
