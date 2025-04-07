using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceNet.Test.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_classification",
                table: "classification");

            migrationBuilder.RenameTable(
                name: "classification",
                newName: "Classification");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classification",
                table: "Classification",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    LegalPersonId = table.Column<long>(type: "bigint", nullable: true),
                    PhysicsPersonId = table.Column<long>(type: "bigint", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_LegalPerson_LegalPersonId",
                        column: x => x.LegalPersonId,
                        principalTable: "LegalPerson",
                        principalColumn: "LegalPersonId");
                    table.ForeignKey(
                        name: "FK_Client_Person_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_PhysicsPerson_PhysicsPersonId",
                        column: x => x.PhysicsPersonId,
                        principalTable: "PhysicsPerson",
                        principalColumn: "PhysicsPersonId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_LegalPersonId",
                table: "Client",
                column: "LegalPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_PhysicsPersonId",
                table: "Client",
                column: "PhysicsPersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classification",
                table: "Classification");

            migrationBuilder.RenameTable(
                name: "Classification",
                newName: "classification");

            migrationBuilder.AddPrimaryKey(
                name: "PK_classification",
                table: "classification",
                column: "Id");
        }
    }
}
