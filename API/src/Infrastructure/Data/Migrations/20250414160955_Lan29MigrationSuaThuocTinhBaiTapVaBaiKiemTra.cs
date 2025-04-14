﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan29MigrationSuaThuocTinhBaiTapVaBaiKiemTra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BaiTap_NgayTao_LichHocId",
                table: "BaiTap");

            migrationBuilder.AlterColumn<Guid>(
                name: "LichHocId",
                table: "BaiKiemTra",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LichHocId",
                table: "BaiKiemTra",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_NgayTao_LichHocId",
                table: "BaiTap",
                columns: new[] { "NgayTao", "LichHocId" },
                unique: true);
        }
    }
}
