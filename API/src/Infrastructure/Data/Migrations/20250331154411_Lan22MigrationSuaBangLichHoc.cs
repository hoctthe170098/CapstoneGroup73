using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan22MigrationSuaBangLichHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_LichHocGocId",
                table: "LichHoc",
                column: "LichHocGocId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_LichHoc_LichHocGocId",
                table: "LichHoc",
                column: "LichHocGocId",
                principalTable: "LichHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_LichHoc_LichHocGocId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_LichHocGocId",
                table: "LichHoc");
        }
    }
}
