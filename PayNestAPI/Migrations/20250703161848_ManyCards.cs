using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayNestAPI.Migrations
{
    /// <inheritdoc />
    public partial class ManyCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                table: "Cards");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                table: "Cards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                table: "Cards");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_UserId",
                table: "Cards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
