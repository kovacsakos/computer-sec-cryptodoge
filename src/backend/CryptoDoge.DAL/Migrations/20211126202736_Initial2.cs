using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoDoge.DAL.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32c21592-1bcf-4aba-8e22-f4b587dc5bd1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c6f3601b-21eb-4d98-b0e1-8fc2fe31e633");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "32c21592-1bcf-4aba-8e22-f4b587dc5bd1", "10d453e2-601d-4347-8bb3-6f29705c099a", "Admin", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c6f3601b-21eb-4d98-b0e1-8fc2fe31e633", "c5bc4d95-d386-4d97-9ada-22725073858a", "User", null });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "52251c06-58a7-4fe4-885d-2a484034326d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ad3e24e3-5187-4c01-aeed-4311e67088b5", "AQAAAAEAACcQAAAAELue407gwUR7OAfwm47KW6737MIxK3GtZoaGHvRtc34hxK+ByMxstvq9S3kk34f8LA==", "de52b4c5-2cb1-4be9-8e6d-c212e1692d73" });
        }
    }
}
