using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadyRuth.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTestimonialImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Testimonials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Testimonials",
                type: "bytea",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$BHeS6EN.Lj/2W5EBK9ToWeXXLeMTEf/Vvlb8n/IViybSh0KwotFsG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Testimonials");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$64VP.PdOWzjYidTTsNkdy.QNT85W2eFj5fn2DQhQ0buOrai0HUPGO");
        }
    }
}
