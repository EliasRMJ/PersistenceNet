using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceNet.Test.Migrations
{
    /// <inheritdoc />
    public partial class ClientStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "classification",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "active",
                table: "classification",
                newName: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "classification",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<int>(
                name: "Active",
                table: "classification",
                type: "int",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 1)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplementName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InclusionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<int>(type: "int", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailPerson",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mail = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailPerson_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalPerson",
                columns: table => new
                {
                    LegalPersonId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    MunicipalRegistration = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalPerson", x => x.LegalPersonId);
                    table.ForeignKey(
                        name: "FK_LegalPerson_Person_LegalPersonId",
                        column: x => x.LegalPersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysicsPerson",
                columns: table => new
                {
                    PhysicsPersonId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    DateBirth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicsPerson", x => x.PhysicsPersonId);
                    table.ForeignKey(
                        name: "FK_PhysicsPerson_Person_PhysicsPersonId",
                        column: x => x.PhysicsPersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailPerson_PersonId",
                table: "EmailPerson",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailPerson");

            migrationBuilder.DropTable(
                name: "LegalPerson");

            migrationBuilder.DropTable(
                name: "PhysicsPerson");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "classification",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "classification",
                newName: "active");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "classification",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<int>(
                name: "active",
                table: "classification",
                type: "int",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 1)
                .Annotation("Relational:ColumnOrder", 3);
        }
    }
}
