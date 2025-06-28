using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Insania.Files.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "insania_files");

            migrationBuilder.CreateTable(
                name: "c_files_types",
                schema: "insania_files",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    path = table.Column<string>(type: "text", nullable: false, comment: "Путь"),
                    date_create = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Дата создания"),
                    username_create = table.Column<string>(type: "text", nullable: false, comment: "Логин пользователя, создавшего"),
                    date_update = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Дата обновления"),
                    username_update = table.Column<string>(type: "text", nullable: false, comment: "Логин пользователя, обновившего"),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Дата удаления"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование"),
                    alias = table.Column<string>(type: "text", nullable: false, comment: "Псевдоним")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_files_types", x => x.id);
                    table.UniqueConstraint("AK_c_files_types_alias", x => x.alias);
                },
                comment: "Типы файлов");

            migrationBuilder.CreateTable(
                name: "r_files",
                schema: "insania_files",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование"),
                    extension = table.Column<string>(type: "text", nullable: false, comment: "Расширение"),
                    type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор типа"),
                    entity_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности"),
                    date_create = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Дата создания"),
                    username_create = table.Column<string>(type: "text", nullable: false, comment: "Логин пользователя, создавшего"),
                    date_update = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Дата обновления"),
                    username_update = table.Column<string>(type: "text", nullable: false, comment: "Логин пользователя, обновившего"),
                    date_deleted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_r_files_c_files_types_type_id",
                        column: x => x.type_id,
                        principalSchema: "insania_files",
                        principalTable: "c_files_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Файлы");

            migrationBuilder.CreateIndex(
                name: "IX_r_files_type_id",
                schema: "insania_files",
                table: "r_files",
                column: "type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "r_files",
                schema: "insania_files");

            migrationBuilder.DropTable(
                name: "c_files_types",
                schema: "insania_files");
        }
    }
}
