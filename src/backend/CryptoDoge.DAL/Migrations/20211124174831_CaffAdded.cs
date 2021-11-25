using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoDoge.DAL.Migrations
{
    public partial class CaffAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caffs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumberOfAnimations = table.Column<int>(type: "int", nullable: false),
                    CreationYear = table.Column<int>(type: "int", nullable: false),
                    CreationMonth = table.Column<int>(type: "int", nullable: false),
                    CreationDay = table.Column<int>(type: "int", nullable: false),
                    CreationHour = table.Column<int>(type: "int", nullable: false),
                    CreationMinute = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaffComments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaffId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaffComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaffComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CaffComments_Caffs_CaffId",
                        column: x => x.CaffId,
                        principalTable: "Caffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ciffs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaffId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ciffs_Caffs_CaffId",
                        column: x => x.CaffId,
                        principalTable: "Caffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CiffTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CiffId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CiffTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CiffTags_Ciffs_CiffId",
                        column: x => x.CiffId,
                        principalTable: "Ciffs",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "72965351-1b5e-44ba-9104-5c3ece9b9fe8", "AQAAAAEAACcQAAAAEP8ATzedUWGnKU9Oeba+3W4iuowe2UcNnArbPyEqjaK16WfV9OEUZJFQrU+YoCvA4A==", "70c1955a-04cb-4df9-910e-71e1a0612687" });

            migrationBuilder.CreateIndex(
                name: "IX_CaffComments_CaffId",
                table: "CaffComments",
                column: "CaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CaffComments_UserId",
                table: "CaffComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ciffs_CaffId",
                table: "Ciffs",
                column: "CaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CiffTags_CiffId",
                table: "CiffTags",
                column: "CiffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaffComments");

            migrationBuilder.DropTable(
                name: "CiffTags");

            migrationBuilder.DropTable(
                name: "Ciffs");

            migrationBuilder.DropTable(
                name: "Caffs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d764d04a-2135-4476-a082-2fb3ee6df563", "AQAAAAEAACcQAAAAEFSOuMEIi0f32Min+3pq488lL2M3uemxuIbNkB9pyrcE+cUFUKUYnt5IqQLomskoQQ==", "00f2797c-687a-4650-b627-980590e888e5" });
        }
    }
}
