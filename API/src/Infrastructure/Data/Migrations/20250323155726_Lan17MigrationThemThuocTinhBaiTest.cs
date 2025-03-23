using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan17MigrationThemThuocTinhBaiTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TieuDe",
                table: "BaiTap",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UrlFile",
                table: "BaiKiemTra",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                table: "BaiKiemTra",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "BaiKiemTra",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TieuDe",
                table: "BaiTap");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "BaiKiemTra");

            migrationBuilder.AlterColumn<string>(
                name: "UrlFile",
                table: "BaiKiemTra",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                table: "BaiKiemTra",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
