using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan23MigrationSuaBangBaiTap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGianBatDau",
                table: "BaiTap");

            migrationBuilder.RenameColumn(
                name: "Ngay",
                table: "BaiTap",
                newName: "NgayTao");

            migrationBuilder.RenameIndex(
                name: "IX_BaiTap_Ngay_LichHocId",
                table: "BaiTap",
                newName: "IX_BaiTap_NgayTao_LichHocId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ThoiGianKetThuc",
                table: "BaiTap",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NgayTao",
                table: "BaiTap",
                newName: "Ngay");

            migrationBuilder.RenameIndex(
                name: "IX_BaiTap_NgayTao_LichHocId",
                table: "BaiTap",
                newName: "IX_BaiTap_Ngay_LichHocId");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "ThoiGianKetThuc",
                table: "BaiTap",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ThoiGianBatDau",
                table: "BaiTap",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
