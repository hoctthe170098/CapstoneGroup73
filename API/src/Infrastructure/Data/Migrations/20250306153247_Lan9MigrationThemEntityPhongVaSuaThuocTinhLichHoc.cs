using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan9MigrationThemEntityPhongVaSuaThuocTinhLichHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_CoSo_CoSoId",
                table: "LichHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_Slot_SlotId",
                table: "LichHoc");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_CoSoId",
                table: "LichHoc");

            migrationBuilder.DropIndex(
                name: "IX_LichHoc_Thu_Phong_SlotId_TrangThai_CoSoId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "CoSoId",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "Phong",
                table: "LichHoc");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "LichHoc",
                newName: "PhongId");

            migrationBuilder.RenameIndex(
                name: "IX_LichHoc_SlotId",
                table: "LichHoc",
                newName: "IX_LichHoc_PhongId");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichHoc",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "NgayKetThuc",
                table: "LichHoc",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GioBatDau",
                table: "LichHoc",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "GioKetThuc",
                table: "LichHoc",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phong_CoSo_CoSoId",
                        column: x => x.CoSoId,
                        principalTable: "CoSo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phong_CoSoId",
                table: "Phong",
                column: "CoSoId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_Phong_PhongId",
                table: "LichHoc",
                column: "PhongId",
                principalTable: "Phong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichHoc_Phong_PhongId",
                table: "LichHoc");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropColumn(
                name: "GioBatDau",
                table: "LichHoc");

            migrationBuilder.DropColumn(
                name: "GioKetThuc",
                table: "LichHoc");

            migrationBuilder.RenameColumn(
                name: "PhongId",
                table: "LichHoc",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_LichHoc_PhongId",
                table: "LichHoc",
                newName: "IX_LichHoc_SlotId");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichHoc",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "NgayKetThuc",
                table: "LichHoc",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<Guid>(
                name: "CoSoId",
                table: "LichHoc",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Phong",
                table: "LichHoc",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    KetThuc = table.Column<TimeOnly>(type: "time", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slot", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_LichHoc_Slot_SlotId",
                table: "LichHoc",
                column: "SlotId",
                principalTable: "Slot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
