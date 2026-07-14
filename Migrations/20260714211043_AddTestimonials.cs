using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LadyRuth.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTestimonials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Testimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Quote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonials", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$64VP.PdOWzjYidTTsNkdy.QNT85W2eFj5fn2DQhQ0buOrai0HUPGO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Testimonials");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$84BV5qHaA6lFCuCvt8LSnuMIv7s1W51cHeusUAngZnfsgvSui3rZ.");
        }
    }
}
