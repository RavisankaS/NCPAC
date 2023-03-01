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
                name: "Announcements",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AnnouncementDescription = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    TimePosted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.ID);
                });

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
                name: "Employees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MailPrefferences",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailPrefferences", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeetingTitle = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MeetingLink = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TimeFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TimeTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CommiteeID = table.Column<int>(type: "INTEGER", nullable: true),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCancelled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Meetings_Commitees_CommiteeID",
                        column: x => x.CommiteeID,
                        principalTable: "Commitees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Salutation = table.Column<string>(type: "TEXT", nullable: true),
                    StreetAddress = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    ProvinceID = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    WorkStreetAddress = table.Column<string>(type: "TEXT", nullable: true),
                    WorkCity = table.Column<string>(type: "TEXT", nullable: true),
                    WorkProvinceID = table.Column<string>(type: "TEXT", nullable: true),
                    WorkPostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    WorkPhone = table.Column<string>(type: "TEXT", nullable: true),
                    WorkEmail = table.Column<string>(type: "TEXT", nullable: true),
                    MailPrefferenceID = table.Column<string>(type: "TEXT", nullable: true),
                    EducationalSummary = table.Column<string>(type: "TEXT", nullable: true),
                    IsNCGrad = table.Column<bool>(type: "INTEGER", nullable: false),
                    OccupationalSummary = table.Column<string>(type: "TEXT", nullable: true),
                    DateJoined = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Members_MailPrefferences_MailPrefferenceID",
                        column: x => x.MailPrefferenceID,
                        principalTable: "MailPrefferences",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Members_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Members_Provinces_WorkProvinceID",
                        column: x => x.WorkProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MemberVM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Salutation = table.Column<string>(type: "TEXT", nullable: true),
                    StreetAddress = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    ProvinceID = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    WorkStreetAddress = table.Column<string>(type: "TEXT", nullable: true),
                    WorkCity = table.Column<string>(type: "TEXT", nullable: true),
                    WorkProvinceID = table.Column<string>(type: "TEXT", nullable: true),
                    WorkPostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    WorkPhone = table.Column<string>(type: "TEXT", nullable: true),
                    WorkEmail = table.Column<string>(type: "TEXT", nullable: true),
                    MailPrefferenceID = table.Column<string>(type: "TEXT", nullable: true),
                    EducationalSummary = table.Column<string>(type: "TEXT", nullable: true),
                    IsNCGrad = table.Column<bool>(type: "INTEGER", nullable: false),
                    OccupationalSummary = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberVM", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MemberVM_MailPrefferences_MailPrefferenceID",
                        column: x => x.MailPrefferenceID,
                        principalTable: "MailPrefferences",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MemberVM_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MemberVM_Provinces_WorkProvinceID",
                        column: x => x.WorkProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    MeetingID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Meetings_MeetingID",
                        column: x => x.MeetingID,
                        principalTable: "Meetings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberCommitees",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    CommiteeID = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberVMID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberCommitees", x => new { x.MemberID, x.CommiteeID });
                    table.ForeignKey(
                        name: "FK_MemberCommitees_Commitees_CommiteeID",
                        column: x => x.CommiteeID,
                        principalTable: "Commitees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberCommitees_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberCommitees_MemberVM_MemberVMID",
                        column: x => x.MemberVMID,
                        principalTable: "MemberVM",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FileContent",
                columns: table => new
                {
                    FileContentID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileContent", x => x.FileContentID);
                    table.ForeignKey(
                        name: "FK_FileContent_UploadedFiles_FileContentID",
                        column: x => x.FileContentID,
                        principalTable: "UploadedFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_CommiteeID",
                table: "Meetings",
                column: "CommiteeID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCommitees_CommiteeID",
                table: "MemberCommitees",
                column: "CommiteeID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCommitees_MemberVMID",
                table: "MemberCommitees",
                column: "MemberVMID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MailPrefferenceID",
                table: "Members",
                column: "MailPrefferenceID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ProvinceID",
                table: "Members",
                column: "ProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_WorkProvinceID",
                table: "Members",
                column: "WorkProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberVM_MailPrefferenceID",
                table: "MemberVM",
                column: "MailPrefferenceID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberVM_ProvinceID",
                table: "MemberVM",
                column: "ProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberVM_WorkProvinceID",
                table: "MemberVM",
                column: "WorkProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_MeetingID",
                table: "UploadedFiles",
                column: "MeetingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "FileContent");

            migrationBuilder.DropTable(
                name: "MemberCommitees");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "MemberVM");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "MailPrefferences");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Commitees");
        }
    }
}
