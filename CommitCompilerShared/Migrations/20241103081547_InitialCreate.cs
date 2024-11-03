using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommitCompilerShared.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Organization = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Project = table.Column<string>(type: "TEXT", nullable: false),
                    Repository = table.Column<string>(type: "TEXT", nullable: false),
                    Branch = table.Column<string>(type: "TEXT", nullable: false),
                    DateStartProcess = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateEndProcess = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessTime = table.Column<double>(type: "REAL", nullable: false),
                    AutoMerge = table.Column<bool>(type: "INTEGER", nullable: false),
                    DestinationBranch = table.Column<string>(type: "TEXT", nullable: false),
                    Notification = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailOriginSender = table.Column<string>(type: "TEXT", nullable: false),
                    EmailOriginPass = table.Column<string>(type: "TEXT", nullable: false),
                    EmailDestination = table.Column<string>(type: "TEXT", nullable: false),
                    EmailDestinationSubject = table.Column<string>(type: "TEXT", nullable: false),
                    NotificationSubjectCommit = table.Column<string>(type: "TEXT", nullable: false),
                    PathDestination = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildConfigurations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildConfigurations");
        }
    }
}
