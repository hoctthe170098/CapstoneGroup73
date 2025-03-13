using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan8MigrationThemKhoaUniqueLichHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai",
                table: "LichHoc");

            migrationBuilder.AddColumn<Guid>(
                name: "CoSoId",
                table: "LichHoc",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_CoSoId",
                table: "LichHoc",
                column: "CoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai_CoSoId",
                table: "LichHoc",
                columns: new[] { "Thu", "Phong", "SlotId", "TrangThai", "CoSoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_CoSo_CoSoId",
                table: "LichHoc",
                column: "CoSoId",
                principalTable: "CoSo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_CoSo_CoSoId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_CoSoId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai_CoSoId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "CoSoId",
                table: "LichHoc");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai",
                table: "LichHoc",
                columns: new[] { "Thu", "Phong", "SlotId", "TrangThai" },
                unique: true);
        }
    }
}
