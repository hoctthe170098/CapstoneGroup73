using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    public DbSet<CoSo> CoSos {  get; }
    public DbSet<ChinhSach> ChinhSaches {  get; }
    public DbSet<BaiTap> HocSinhs {  get; }
    public DbSet<GiaoVien> GiaoViens {  get; }
    public DbSet<NhanVien> NhanViens {  get; }
    public DbSet<Slot> Slots {  get; }
    public DbSet<ChuongTrinh> ChuongTrinhs {  get; }
    public DbSet<LichHoc> LichHocs {  get; }
    public DbSet<ThamGiaLopHoc> ThamGiaLopHocs {  get; }
    public DbSet<DiemDanh> DiemDanhs {  get; }
    public DbSet<BaiTap> BaiTaps {  get; }
    public DbSet<TraLoi> TraLois {  get; }
    public DbSet<BaiTap> BaiKiemTras {  get; }
    public DbSet<KetQuaBaiKiemTra> KetQuaBaiKiemTras { get; }
    public DbSet<NoiDungBaiHoc> NoiDungBaiHocs {  get; }
    public DbSet<TaiLieuHocTap> TaiLieuHocTaps {  get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
