using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoDoge.DAL.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fcb158b-f714-46cb-8faf-16883ab592fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f83ae1dd-fb0c-446a-a734-ccde51cfb27e");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "7fcb158b-f714-46cb-8faf-16883ab592fe", "06de3e24-f2f1-488d-8dc8-da5a8ccb6862", "ADMIN", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f83ae1dd-fb0c-446a-a734-ccde51cfb27e", "7b9465c2-4c4a-4c2d-9031-b18980f681dc", "USER", null });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e438bee-e648-4302-8614-c4227f835745", "AQAAAAEAACcQAAAAEBt5mhRTVDndah6o/96K/LwfbgVUjzMdp+NpmOryvhoTWMPFbFRHE1oLTc+IfuwIYw==", "5772be96-4ea1-4c44-bb37-c1f161ec013d" });
        }
    }
}
