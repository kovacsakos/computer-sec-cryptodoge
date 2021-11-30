using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoDoge.DAL.Migrations
{
    public partial class CommentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Added",
                table: "CaffComments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34790367-a250-442c-bf25-2adbcf3f39a4",
                column: "ConcurrencyStamp",
                value: "7d7a7916-eb70-40de-9a66-234f0d58dfb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59381279-94ad-4e44-9817-d0c46350464a",
                column: "ConcurrencyStamp",
                value: "c6584541-e786-4821-a6c4-6b4639ae52be");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3475dbb0-2a20-423f-9191-02b8d7cfee88", "AQAAAAEAACcQAAAAEFNttxaihr6nWw8ogBWs7UVtsj1QZ3RqWs6Hll9CdnKLBhQ0+wstBY9sCuxeVzQo7g==", "7fe8e3b7-80d9-4e3d-8aa7-499f781ad761" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcc978f4-aeb6-435f-8fa9-09f6c97735c9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0516812c-92a8-4b1d-bc77-8c625fad780b", "AQAAAAEAACcQAAAAELEc8+9on/86MfXLDFbqIoNdYr+whZ3t+QDfX/zZdKu82zyZ5BfwwCQewFHZ4KmYig==", "3f433a43-786a-40c0-b74f-c73dd6d9d2d8" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Added",
                table: "CaffComments");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34790367-a250-442c-bf25-2adbcf3f39a4",
                column: "ConcurrencyStamp",
                value: "2274aaff-d2ca-4d11-9220-f5d0e80b9abf");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59381279-94ad-4e44-9817-d0c46350464a",
                column: "ConcurrencyStamp",
                value: "22718be9-6b9f-45ad-9bf6-3d89a1496763");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61660001-62d2-4806-a9b0-b7141bfe4d99", "AQAAAAEAACcQAAAAEPi0Z7mU6IKPyG9W2A6GJs/TQmunLIXIDKWfaK/qB/Q2OjPpibENPwpuZzj7I1gqGQ==", "f16b3854-aa8e-4213-b70c-8a5402efb583" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcc978f4-aeb6-435f-8fa9-09f6c97735c9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9823fc53-0a21-4082-be6d-c4f31cf7ceed", "AQAAAAEAACcQAAAAENzwO+oM9CzflEr5sjQzfCVBnvdklkmMND+oB0jkwLxCsK7zK0d5sWoOk+KI+WBahg==", "545bec4d-f190-4546-9fe8-42dd683a8dce" });
        }
    }
}
