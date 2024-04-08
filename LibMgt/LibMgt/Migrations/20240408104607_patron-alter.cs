using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibMgt.Migrations
{
    /// <inheritdoc />
    public partial class patronalter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_Patrons_PatronID",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Patrons_PatronID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Patrons_PatronID",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_AspNetUsers_PatronID",
                table: "Fines",
                column: "PatronID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_PatronID",
                table: "Reservations",
                column: "PatronID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_PatronID",
                table: "Transactions",
                column: "PatronID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_AspNetUsers_PatronID",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_PatronID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_PatronID",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_Patrons_PatronID",
                table: "Fines",
                column: "PatronID",
                principalTable: "Patrons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Patrons_PatronID",
                table: "Reservations",
                column: "PatronID",
                principalTable: "Patrons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Patrons_PatronID",
                table: "Transactions",
                column: "PatronID",
                principalTable: "Patrons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
