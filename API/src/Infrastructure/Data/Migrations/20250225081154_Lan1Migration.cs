using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lan1Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TodoLists_ListId",
                table: "TodoItems");

            migrationBuilder.CreateTable(
                name: "ChinhSach",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Mota = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhanTramGiam = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChinhSach", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChuongTrinh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuongTrinh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoSo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoSo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    KetThuc = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoiDungBaiHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mota = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChuongTrinhId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiDungBaiHoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoiDungBaiHoc_ChuongTrinh_ChuongTrinhId",
                        column: x => x.ChuongTrinhId,
                        principalTable: "ChuongTrinh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GiaoVien",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TruongDangDay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoVien", x => x.Code);
                    table.ForeignKey(
                        name: "FK_GiaoVien_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GiaoVien_CoSo_CoSoId",
                        column: x => x.CoSoId,
                        principalTable: "CoSo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HocSinh",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TruongDangHoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChinhSachId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocSinh", x => x.Code);
                    table.ForeignKey(
                        name: "FK_HocSinh_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HocSinh_ChinhSach_ChinhSachId",
                        column: x => x.ChinhSachId,
                        principalTable: "ChinhSach",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HocSinh_CoSo_CoSoId",
                        column: x => x.CoSoId,
                        principalTable: "CoSo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoSoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.Code);
                    table.ForeignKey(
                        name: "FK_NhanVien_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhanVien_CoSo_CoSoId",
                        column: x => x.CoSoId,
                        principalTable: "CoSo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaiLieuHocTap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false),
                    urlType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    urlFile = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDungBaiHocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiLieuHocTap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiLieuHocTap_NoiDungBaiHoc_NoiDungBaiHocId",
                        column: x => x.NoiDungBaiHocId,
                        principalTable: "NoiDungBaiHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Thu = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    Phong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenLop = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayBatDau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayKetThuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HocPhi = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaoVienCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ChuongTrinhId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichHoc_ChuongTrinh_ChuongTrinhId",
                        column: x => x.ChuongTrinhId,
                        principalTable: "ChuongTrinh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHoc_GiaoVien_GiaoVienCode",
                        column: x => x.GiaoVienCode,
                        principalTable: "GiaoVien",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHoc_Slot_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaiKiemTra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UrlFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayKiemTra = table.Column<DateOnly>(type: "date", nullable: true),
                    LichHocId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiKiemTra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaiKiemTra_LichHoc_LichHocId",
                        column: x => x.LichHocId,
                        principalTable: "LichHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaiTap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    LichHocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(750)", maxLength: 750, nullable: false),
                    ThoiGianBatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    ThoiGianKetThuc = table.Column<TimeOnly>(type: "time", nullable: false),
                    UrlFile = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ChinhSachId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiTap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaiTap_ChinhSach_ChinhSachId",
                        column: x => x.ChinhSachId,
                        principalTable: "ChinhSach",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaiTap_LichHoc_LichHocId",
                        column: x => x.LichHocId,
                        principalTable: "LichHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThamGiaLopHoc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayBatDau = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayKetThuc = table.Column<DateOnly>(type: "date", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HocSinhCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    LichHocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamGiaLopHoc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThamGiaLopHoc_HocSinh_HocSinhCode",
                        column: x => x.HocSinhCode,
                        principalTable: "HocSinh",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThamGiaLopHoc_LichHoc_LichHocId",
                        column: x => x.LichHocId,
                        principalTable: "LichHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaBaiKiemTra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HocSinhCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    BaiKiemTraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Diem = table.Column<float>(type: "real", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaBaiKiemTra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaBaiKiemTra_BaiKiemTra_BaiKiemTraId",
                        column: x => x.BaiKiemTraId,
                        principalTable: "BaiKiemTra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KetQuaBaiKiemTra_HocSinh_HocSinhCode",
                        column: x => x.HocSinhCode,
                        principalTable: "HocSinh",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TraLoi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(750)", maxLength: 750, nullable: false),
                    UrlFile = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HocSinhCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    BaiTapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraLoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraLoi_BaiTap_BaiTapId",
                        column: x => x.BaiTapId,
                        principalTable: "BaiTap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraLoi_HocSinh_HocSinhCode",
                        column: x => x.HocSinhCode,
                        principalTable: "HocSinh",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiemDanh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    ThamGiaLopHocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiemBTVN = table.Column<float>(type: "real", nullable: true),
                    DiemTrenLop = table.Column<float>(type: "real", nullable: true),
                    NhanXet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemDanh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiemDanh_ThamGiaLopHoc_ThamGiaLopHocId",
                        column: x => x.ThamGiaLopHocId,
                        principalTable: "ThamGiaLopHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiKiemTra_LichHocId",
                table: "BaiKiemTra",
                column: "LichHocId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_ChinhSachId",
                table: "BaiTap",
                column: "ChinhSachId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_LichHocId",
                table: "BaiTap",
                column: "LichHocId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_Ngay_LichHocId",
                table: "BaiTap",
                columns: new[] { "Ngay", "LichHocId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiemDanh_Ngay_ThamGiaLopHocId",
                table: "DiemDanh",
                columns: new[] { "Ngay", "ThamGiaLopHocId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiemDanh_ThamGiaLopHocId",
                table: "DiemDanh",
                column: "ThamGiaLopHocId");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVien_CoSoId",
                table: "GiaoVien",
                column: "CoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoVien_UserId",
                table: "GiaoVien",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HocSinh_ChinhSachId",
                table: "HocSinh",
                column: "ChinhSachId");

            migrationBuilder.CreateIndex(
                name: "IX_HocSinh_CoSoId",
                table: "HocSinh",
                column: "CoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_HocSinh_UserId",
                table: "HocSinh",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaBaiKiemTra_BaiKiemTraId",
                table: "KetQuaBaiKiemTra",
                column: "BaiKiemTraId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaBaiKiemTra_HocSinhCode_BaiKiemTraId",
                table: "KetQuaBaiKiemTra",
                columns: new[] { "HocSinhCode", "BaiKiemTraId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_ChuongTrinhId",
                table: "LichHoc",
                column: "ChuongTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_GiaoVienCode",
                table: "LichHoc",
                column: "GiaoVienCode");

            migrationBuilder.CreateIndex(
                name: "IX_LichHoc_SlotId",
                table: "LichHoc",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_CoSoId",
                table: "NhanVien",
                column: "CoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_UserId",
                table: "NhanVien",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NoiDungBaiHoc_ChuongTrinhId",
                table: "NoiDungBaiHoc",
                column: "ChuongTrinhId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiLieuHocTap_NoiDungBaiHocId",
                table: "TaiLieuHocTap",
                column: "NoiDungBaiHocId");

            migrationBuilder.CreateIndex(
                name: "IX_ThamGiaLopHoc_HocSinhCode_LichHocId",
                table: "ThamGiaLopHoc",
                columns: new[] { "HocSinhCode", "LichHocId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThamGiaLopHoc_LichHocId",
                table: "ThamGiaLopHoc",
                column: "LichHocId");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoi_BaiTapId",
                table: "TraLoi",
                column: "BaiTapId");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoi_HocSinhCode",
                table: "TraLoi",
                column: "HocSinhCode");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TodoLists_ListId",
                table: "TodoItems",
                column: "ListId",
                principalTable: "TodoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TodoLists_ListId",
                table: "TodoItems");

            migrationBuilder.DropTable(
                name: "DiemDanh");

            migrationBuilder.DropTable(
                name: "KetQuaBaiKiemTra");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "TaiLieuHocTap");

            migrationBuilder.DropTable(
                name: "TraLoi");

            migrationBuilder.DropTable(
                name: "ThamGiaLopHoc");

            migrationBuilder.DropTable(
                name: "BaiKiemTra");

            migrationBuilder.DropTable(
                name: "NoiDungBaiHoc");

            migrationBuilder.DropTable(
                name: "BaiTap");

            migrationBuilder.DropTable(
                name: "HocSinh");

            migrationBuilder.DropTable(
                name: "LichHoc");

            migrationBuilder.DropTable(
                name: "ChinhSach");

            migrationBuilder.DropTable(
                name: "ChuongTrinh");

            migrationBuilder.DropTable(
                name: "GiaoVien");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "CoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TodoLists_ListId",
                table: "TodoItems",
                column: "ListId",
                principalTable: "TodoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
