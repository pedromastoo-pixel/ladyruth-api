using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadyRuth.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPayFastPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayFastPaymentId",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$84BV5qHaA6lFCuCvt8LSnuMIv7s1W51cHeusUAngZnfsgvSui3rZ.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayFastPaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$H7bMqn2dSSBZmtdOU5DpDOf3I8H06mXujbeCcOCPgSfLmTFSuc6/.");
        }
    }
}
