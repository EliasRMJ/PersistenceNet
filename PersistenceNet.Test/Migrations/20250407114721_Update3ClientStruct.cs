using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceNet.Test.Migrations
{
    /// <inheritdoc />
    public partial class Update3ClientStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClassificationId",
                table: "Client",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Client_ClassificationId",
                table: "Client",
                column: "ClassificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Classification_ClassificationId",
                table: "Client",
                column: "ClassificationId",
                principalTable: "Classification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Classification_ClassificationId",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_ClassificationId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "ClassificationId",
                table: "Client");
        }
    }
}
