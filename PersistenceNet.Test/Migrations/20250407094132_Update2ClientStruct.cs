using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceNet.Test.Migrations
{
    /// <inheritdoc />
    public partial class Update2ClientStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_LegalPerson_LegalPersonId",
                table: "Client");

            migrationBuilder.DropForeignKey(
                name: "FK_Client_PhysicsPerson_PhysicsPersonId",
                table: "Client");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalPerson_Person_LegalPersonId",
                table: "LegalPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicsPerson_Person_PhysicsPersonId",
                table: "PhysicsPerson");

            migrationBuilder.DropIndex(
                name: "IX_Client_LegalPersonId",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_PhysicsPersonId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "LegalPersonId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "PhysicsPersonId",
                table: "Client");

            migrationBuilder.RenameColumn(
                name: "PhysicsPersonId",
                table: "PhysicsPerson",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LegalPersonId",
                table: "LegalPerson",
                newName: "Id");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PhysicsPerson",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LegalPerson",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalPerson_Person_Id",
                table: "LegalPerson",
                column: "Id",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicsPerson_Person_Id",
                table: "PhysicsPerson",
                column: "Id",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalPerson_Person_Id",
                table: "LegalPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicsPerson_Person_Id",
                table: "PhysicsPerson");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PhysicsPerson",
                newName: "PhysicsPersonId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LegalPerson",
                newName: "LegalPersonId");

            migrationBuilder.AlterColumn<long>(
                name: "PhysicsPersonId",
                table: "PhysicsPerson",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<long>(
                name: "LegalPersonId",
                table: "LegalPerson",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<long>(
                name: "LegalPersonId",
                table: "Client",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PhysicsPersonId",
                table: "Client",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Client_LegalPersonId",
                table: "Client",
                column: "LegalPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_PhysicsPersonId",
                table: "Client",
                column: "PhysicsPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_LegalPerson_LegalPersonId",
                table: "Client",
                column: "LegalPersonId",
                principalTable: "LegalPerson",
                principalColumn: "LegalPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_PhysicsPerson_PhysicsPersonId",
                table: "Client",
                column: "PhysicsPersonId",
                principalTable: "PhysicsPerson",
                principalColumn: "PhysicsPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalPerson_Person_LegalPersonId",
                table: "LegalPerson",
                column: "LegalPersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicsPerson_Person_PhysicsPersonId",
                table: "PhysicsPerson",
                column: "PhysicsPersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
