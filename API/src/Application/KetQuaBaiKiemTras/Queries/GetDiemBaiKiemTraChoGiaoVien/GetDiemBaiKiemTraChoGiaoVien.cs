using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.KetQuaBaiKiemTras.Queries.GetDiemBaiKiemTraChoGiaoVien;
public record GetDiemBaiKiemTraChoGiaoVienQuery : IRequest<Output>
{
    public required string BaiKiemTraId { get; init; }
}
public class GetDiemBaiKiemTraChoGiaoVienQueryHandler : IRequestHandler<GetDiemBaiKiemTraChoGiaoVienQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetDiemBaiKiemTraChoGiaoVienQueryHandler(IApplicationDbContext context
        , IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _mapper = mapper;
    }
    public async Task<Output> Handle(GetDiemBaiKiemTraChoGiaoVienQuery request, CancellationToken cancellationToken)
    {
        Output output = new Output
        {
            isError = false,
            code = 200,
            message = "Lấy kết quả bài kiểm tra thành công."
        };
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var UserId = _identityService.GetUserId(token);
        var coSoId = _identityService.GetCampusId(token);
        if (UserId == Guid.Empty) throw new NotFoundIDException();
        var giaoVien = await _context.GiaoViens
            .FirstOrDefaultAsync(gv => gv.UserId == UserId.ToString());
        if (giaoVien == null) throw new NotFoundIDException();
        var HomNay = DateOnly.FromDateTime(DateTime.Now);
        var BaiKiemTra = _context.BaiKiemTras
            .Include(kt=>kt.LichHoc)
            .FirstOrDefault(kt=>kt.Id.ToString()==request.BaiKiemTraId);
        if (BaiKiemTra == null) throw new NotFoundIDException();
        if(BaiKiemTra.LichHoc.GiaoVienCode!=giaoVien.Code) throw new NotFoundIDException();
        if (BaiKiemTra.NgayKiemTra >= HomNay) throw new Exception("Chưa đến ngày kiểm tra, không thể cập nhật kết quả.");
        var checkKetQua = _context.KetQuaBaiKiemTras.Any(kq=>kq.BaiKiemTraId==BaiKiemTra.Id);
        if (!checkKetQua)
        {
            var HocSinhCodes = _context.ThamGiaLopHocs
                .Where(tg=>tg.NgayKetThuc>=BaiKiemTra.NgayKiemTra&&tg.LichHocId==BaiKiemTra.LichHocId)
                .Select(tg=>tg.HocSinhCode).ToList();
            foreach(var code in HocSinhCodes)
            {
                var ketQua = new KetQuaBaiKiemTra
                {
                    BaiKiemTraId = BaiKiemTra.Id,
                    Id = Guid.NewGuid(),
                    HocSinhCode = code,
                };
                _context.KetQuaBaiKiemTras.Add(ketQua);
            }
            BaiKiemTra.TrangThai = "Đã kiểm tra";
        }
        await _context.SaveChangesAsync(cancellationToken);
        var KetQua = await _context.KetQuaBaiKiemTras
            .Where(kq => kq.BaiKiemTraId == BaiKiemTra.Id)
            .ProjectTo<KetQuaBaiKiemTraDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        var data = new
        {
            TenBaiKiemTra = BaiKiemTra.Ten,
            BaiKiemTra.NgayKiemTra,
            KetQuaBaiKiemTra = KetQua
        };
        output.data = data;
        return output;
    }
}
