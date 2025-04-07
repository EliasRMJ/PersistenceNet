using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceNet.Test.Migrations
{
    /// <inheritdoc />
    public partial class Update5ClientStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Classification_ClassificationId",
                table: "Client");

            migrationBuilder.AlterColumn<long>(
                name: "ClassificationId",
                table: "Client",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Classification_ClassificationId",
                table: "Client",
                column: "ClassificationId",
                principalTable: "Classification",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Classification_ClassificationId",
                table: "Client");

            migrationBuilder.AlterColumn<long>(
                name: "ClassificationId",
                table: "Client",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Classification_ClassificationId",
                table: "Client",
                column: "ClassificationId",
                principalTable: "Classification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
