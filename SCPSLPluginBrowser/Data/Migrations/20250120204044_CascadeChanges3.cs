using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCPSLPluginBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class CascadeChanges3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DllFiles_DllFileId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Flags_AspNetUsers_UserId",
                table: "Flags");

            migrationBuilder.DropForeignKey(
                name: "FK_Flags_DllFiles_DllFileId",
                table: "Flags");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_DllFiles_DllFileId",
                table: "Likes");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DllFiles_DllFileId",
                table: "Comments",
                column: "DllFileId",
                principalTable: "DllFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Flags_AspNetUsers_UserId",
                table: "Flags",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Flags_DllFiles_DllFileId",
                table: "Flags",
                column: "DllFileId",
                principalTable: "DllFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_DllFiles_DllFileId",
                table: "Likes",
                column: "DllFileId",
                principalTable: "DllFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DllFiles_DllFileId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Flags_AspNetUsers_UserId",
                table: "Flags");

            migrationBuilder.DropForeignKey(
                name: "FK_Flags_DllFiles_DllFileId",
                table: "Flags");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_DllFiles_DllFileId",
                table: "Likes");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DllFiles_DllFileId",
                table: "Comments",
                column: "DllFileId",
                principalTable: "DllFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Flags_AspNetUsers_UserId",
                table: "Flags",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Flags_DllFiles_DllFileId",
                table: "Flags",
                column: "DllFileId",
                principalTable: "DllFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_DllFiles_DllFileId",
                table: "Likes",
                column: "DllFileId",
                principalTable: "DllFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
