using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NCPAC_LambdaX.Data.NCPACMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commitees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommiteeName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Division = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commitees", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Salutation = table.Column<string>(type: "TEXT", nullable: true),
                    StreetAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    WorkStreetAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WorkCity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    WorkProvince = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    WorkPostalCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: true),
                    WorkPhone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    WorkEmail = table.Column<string>(type: "TEXT", nullable: true),
                    PrefferedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    EducationalSummary = table.Column<string>(type: "TEXT", nullable: true),
                    IsNCGrad = table.Column<bool>(type: "INTEGER", nullable: false),
                    OccupationalSummary = table.Column<string>(type: "TEXT", nullable: true),
                    DateJoined = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CommiteeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Members_Commitees_CommiteeID",
                        column: x => x.CommiteeID,
                        principalTable: "Commitees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_CommiteeID",
                table: "Members",
                column: "CommiteeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Commitees");
        }
    }
}
