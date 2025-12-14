using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextAdventureApi.Migrations
{
    /// <inheritdoc />
    public partial class keyshare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Share = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinRole = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyShares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    IsLockedOut = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "KeyShares",
                columns: new[] { "Id", "MinRole", "RoomId", "Share" },
                values: new object[] { new Guid("04726e2d-23f4-4ce8-982b-0eff1f327f01"), "Player", "main", "ABC-EFG-HIJK" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyShares");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
