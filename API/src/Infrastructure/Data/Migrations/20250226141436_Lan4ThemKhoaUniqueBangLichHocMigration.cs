using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan4ThemKhoaUniqueBangLichHocMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichHoc",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai",
                table: "LichHoc",
                columns: new[] { "Thu", "Phong", "SlotId", "TrangThai" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai",
                table: "LichHoc");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichHoc",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
