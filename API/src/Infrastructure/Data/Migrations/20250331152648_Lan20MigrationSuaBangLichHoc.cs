using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan20MigrationSuaBangLichHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LichHocGocId",
                table: "LichHoc",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LichHocId",
                table: "LichHoc",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NgayHocGoc",
                table: "LichHoc",
                type: "date",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_LichHocId",
                table: "LichHoc",
                column: "LichHocId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_LichHoc_LichHocId",
                table: "LichHoc",
                column: "LichHocId",
                principalTable: "LichHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_LichHoc_LichHocId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_LichHocId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "LichHocGocId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "LichHocId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "NgayHocGoc",
                table: "LichHoc");
        }
    }
}
