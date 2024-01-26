using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteWebApp.Migrations
{
    /// <inheritdoc />
    public partial class up3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_AppUseId",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "AppUseId",
                table: "Notes",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notes_AppUseId",
                table: "Notes",
                newName: "IX_Notes_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_AppUserId",
                table: "Notes",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_AppUserId",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Notes",
                newName: "AppUseId");

            migrationBuilder.RenameIndex(
                name: "IX_Notes_AppUserId",
                table: "Notes",
                newName: "IX_Notes_AppUseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_AppUseId",
                table: "Notes",
                column: "AppUseId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
