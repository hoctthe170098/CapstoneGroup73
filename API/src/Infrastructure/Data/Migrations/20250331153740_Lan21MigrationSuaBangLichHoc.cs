using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan21MigrationSuaBangLichHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_LichHoc_LichHocId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_LichHocId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "LichHocId",
                table: "LichHoc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LichHocId",
                table: "LichHoc",
                type: "uniqueidentifier",
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
    }
}
