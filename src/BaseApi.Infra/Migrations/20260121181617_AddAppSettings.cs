using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApi.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "Base API"),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#3B82F6"),
                    SecondaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#8B5CF6"),
                    AccentColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#22C55E"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");
        }
    }
}
