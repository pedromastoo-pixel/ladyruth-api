using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadyRuth.API.Migrations
{
    /// <inheritdoc />
    public partial class ImageStoredInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductImages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "ProductImages",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$H7bMqn2dSSBZmtdOU5DpDOf3I8H06mXujbeCcOCPgSfLmTFSuc6/.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ProductImages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$MPHat3KXVucw50BA7xLy5uQE.MxQdXEV4pOk30h5bQ4GgwCK8UTfC");
        }
    }
}
