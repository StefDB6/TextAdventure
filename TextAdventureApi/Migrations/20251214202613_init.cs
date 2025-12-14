using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextAdventureApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
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
                values: new object[] { new Guid("0433c9ce-f0ed-41d0-acc8-14a5ab1e5f76"), "Player", "main", "ABC-EFG-HIJK" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FailedLoginAttempts", "IsLockedOut", "PasswordHash", "Role", "Username" },
                values: new object[] { new Guid("b659c375-b559-45e1-a83f-5e5b6b2a0801"), 0, false, "8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918", 1, "admin" });
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
