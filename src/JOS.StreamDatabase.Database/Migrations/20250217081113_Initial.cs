using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOS.StreamDatabase.Database.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "real_estate",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_real_estate", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "real_estate_images",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                data = table.Column<byte[]>(type: "bytea", nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                metadata = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_real_estate_images", x => x.id);
                table.ForeignKey(
                    name: "fk_real_estate_images_real_estate_real_estate_id",
                    column: x => x.real_estate_id,
                    principalTable: "real_estate",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_real_estate_images_real_estate_id",
            table: "real_estate_images",
            column: "real_estate_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "real_estate_images");

        migrationBuilder.DropTable(
            name: "real_estate");
    }
}
