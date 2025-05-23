﻿using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<CoSo> CoSos {  get; }
    public DbSet<ChinhSach> ChinhSaches {  get; }
    public DbSet<HocSinh> HocSinhs {  get; }
    public DbSet<GiaoVien> GiaoViens {  get; }
    public DbSet<NhanVien> NhanViens {  get; }
    public DbSet<Phong> Phongs {  get; }
    public DbSet<ChuongTrinh> ChuongTrinhs {  get; }
    public DbSet<LichHoc> LichHocs {  get; }
    public DbSet<ThamGiaLopHoc> ThamGiaLopHocs {  get; }
    public DbSet<DiemDanh> DiemDanhs {  get; }
    public DbSet<BaiTap> BaiTaps {  get; }
    public DbSet<TraLoi> TraLois {  get; }
    public DbSet<BaiKiemTra> BaiKiemTras {  get; }
    public DbSet<KetQuaBaiKiemTra> KetQuaBaiKiemTras { get; }
    public DbSet<NoiDungBaiHoc> NoiDungBaiHocs {  get; }
    public DbSet<TaiLieuHocTap> TaiLieuHocTaps {  get; }
    public DbSet<NhanXetDinhKy> NhanXetDinhKys {  get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
