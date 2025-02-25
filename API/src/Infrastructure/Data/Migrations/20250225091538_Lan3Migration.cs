using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan3Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaiTap_ChinhSach_ChinhSachId",
                table: "BaiTap");

            migrationBuilder.DropIndex(
                name: "IX_BaiTap_ChinhSachId",
                table: "BaiTap");

            migrationBuilder.DropColumn(
                name: "ChinhSachId",
                table: "BaiTap");

            migrationBuilder.AlterColumn<int>(
                name: "ChinhSachId",
                table: "HocSinh",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ChinhSachId",
                table: "HocSinh",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChinhSachId",
                table: "BaiTap",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_ChinhSachId",
                table: "BaiTap",
                column: "ChinhSachId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaiTap_ChinhSach_ChinhSachId",
                table: "BaiTap",
                column: "ChinhSachId",
                principalTable: "ChinhSach",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
