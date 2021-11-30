using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoDoge.DAL.Migrations
{
    public partial class DataSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09d05665-9d91-4f57-84f6-9e69efaca56d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8094f000-68d0-49e6-8513-3709085b49da");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "59381279-94ad-4e44-9817-d0c46350464a", "22718be9-6b9f-45ad-9bf6-3d89a1496763", "ADMIN", "ADMIN" },
                    { "34790367-a250-442c-bf25-2adbcf3f39a4", "2274aaff-d2ca-4d11-9220-f5d0e80b9abf", "USER", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61660001-62d2-4806-a9b0-b7141bfe4d99", "AQAAAAEAACcQAAAAEPi0Z7mU6IKPyG9W2A6GJs/TQmunLIXIDKWfaK/qB/Q2OjPpibENPwpuZzj7I1gqGQ==", "f16b3854-aa8e-4213-b70c-8a5402efb583" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "bcc978f4-aeb6-435f-8fa9-09f6c97735c9", 0, "9823fc53-0a21-4082-be6d-c4f31cf7ceed", "admin@mail.com", false, false, null, "admin@mail.com", "Admin User", "AQAAAAEAACcQAAAAENzwO+oM9CzflEr5sjQzfCVBnvdklkmMND+oB0jkwLxCsK7zK0d5sWoOk+KI+WBahg==", null, false, null, "545bec4d-f190-4546-9fe8-42dd683a8dce", false, "Admin User" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "59381279-94ad-4e44-9817-d0c46350464a", "bcc978f4-aeb6-435f-8fa9-09f6c97735c9" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "34790367-a250-442c-bf25-2adbcf3f39a4", "52251c06-58a7-4fe4-885d-2a484034326d" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "34790367-a250-442c-bf25-2adbcf3f39a4", "52251c06-58a7-4fe4-885d-2a484034326d" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "59381279-94ad-4e44-9817-d0c46350464a", "bcc978f4-aeb6-435f-8fa9-09f6c97735c9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34790367-a250-442c-bf25-2adbcf3f39a4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59381279-94ad-4e44-9817-d0c46350464a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcc978f4-aeb6-435f-8fa9-09f6c97735c9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "09d05665-9d91-4f57-84f6-9e69efaca56d", "b9cc85ab-b2ff-4593-b2ef-c035cae79b4a", "ADMIN", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8094f000-68d0-49e6-8513-3709085b49da", "85f3a04b-2aa9-479e-9fed-66a21ca8817d", "USER", "USER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2cd5aa90-1677-4c4b-ab99-2320be680491", "AQAAAAEAACcQAAAAEKV3URihJdT4I6G5By/gHWDpiSke47Yx7fdvk6ZsXpiOj7TN26RgUb9tUz+C5RcwSg==", "ecf7ea14-b6ed-4d66-99c3-6f10cfe9ae31" });
        }
    }
}
