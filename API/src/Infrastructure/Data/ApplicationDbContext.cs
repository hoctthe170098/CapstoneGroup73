using System.Reflection;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;
using StudyFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace StudyFlow.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<CoSo> CoSos => Set<CoSo>();
    public DbSet<ChinhSach> ChinhSaches => Set<ChinhSach>();
    public DbSet<BaiTap> HocSinhs => Set<BaiTap>();
    public DbSet<GiaoVien> GiaoViens => Set<GiaoVien>();
    public DbSet<NhanVien> NhanViens => Set<NhanVien>();
    public DbSet<Slot> Slots => Set<Slot>();
    public DbSet<ChuongTrinh> ChuongTrinhs => Set<ChuongTrinh>();
    public DbSet<LichHoc> LichHocs => Set<LichHoc>();
    public DbSet<ThamGiaLopHoc> ThamGiaLopHocs => Set<ThamGiaLopHoc>();
    public DbSet<DiemDanh> DiemDanhs => Set<DiemDanh>();
    public DbSet<BaiTap> BaiTaps => Set<BaiTap>();
    public DbSet<TraLoi> TraLois => Set<TraLoi>();
    public DbSet<BaiTap> BaiKiemTras => Set<BaiTap>();
    public DbSet<KetQuaBaiKiemTra> KetQuaBaiKiemTras => Set<KetQuaBaiKiemTra>();
    public DbSet<NoiDungBaiHoc> NoiDungBaiHocs => Set<NoiDungBaiHoc>();
    public DbSet<TaiLieuHocTap> TaiLieuHocTaps => Set<TaiLieuHocTap>();

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<GiaoVien>()
            .HasOne<ApplicationUser>()
            .WithOne() 
            .HasForeignKey<GiaoVien>(g => g.UserId);

        builder.Entity<HocSinh>()
            .HasOne<ApplicationUser>()
            .WithOne() 
            .HasForeignKey<HocSinh>(h => h.UserId);

        builder.Entity<NhanVien>()
            .HasOne<ApplicationUser>()
            .WithOne() 
             .HasForeignKey<NhanVien>(n => n.UserId);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict; // Hoặc DeleteBehavior.NoAction
        }
    }
}
