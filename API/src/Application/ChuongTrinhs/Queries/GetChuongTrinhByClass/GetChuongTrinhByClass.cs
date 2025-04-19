using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhById;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhByClass;
public record GetChuongTrinhByClassQuery : IRequest<Output>
{
    public required string TenLop { get; init; }
}

public class GetChuongTrinhByClassQueryHandler : IRequestHandler<GetChuongTrinhByClassQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public GetChuongTrinhByClassQueryHandler(IApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    public async Task<Output> Handle(GetChuongTrinhByClassQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
            var userId = _identityService.GetUserId(token);

            var role = await _identityService.GetRolesByUserId(userId.ToString());

            if (role.Contains("Teacher"))
            {
                var giaoVien = await _context.GiaoViens
                    .FirstOrDefaultAsync(gv => gv.UserId == userId.ToString(), cancellationToken);

                if (giaoVien == null)
                    throw new Exception("Không tìm thấy giáo viên tương ứng với tài khoản.");

                var isTeachingClass = await _context.LichHocs
                    .AnyAsync(ld => ld.GiaoVienCode == giaoVien.Code && ld.TenLop == request.TenLop, cancellationToken);

                if (!isTeachingClass)
                    throw new NotFoundIDException();
            }
            else if (role.Contains("Student"))
            {
                var hocSinh = await _context.HocSinhs
                   .FirstOrDefaultAsync(gv => gv.UserId == userId.ToString(), cancellationToken);

                if (hocSinh == null)
                    throw new Exception("Không tìm thấy học sinh tương ứng với tài khoản.");

                var isEnrolledClass = await _context.ThamGiaLopHocs
                    .Include(t => t.LichHoc)
                    .ThenInclude(lh => lh.ChuongTrinh)
                    .FirstOrDefaultAsync(t => t.HocSinhCode == hocSinh.Code && t.LichHoc.TenLop == request.TenLop, cancellationToken);

                if(isEnrolledClass == null)
                {
                    throw new NotFoundIDException();
                }
            }
            else
            {
                throw new AuthenticationException($"Role không được phép: [{string.Join(", ", role)}]");
            }

            var chuongTrinh = await _context.LichHocs
                .Include(ct => ct.ChuongTrinh)
                .Where(lh => lh.TenLop == request.TenLop)
                .Select(lh => lh.ChuongTrinh)
                .ProjectTo<ChuongTrinhDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return new Output
            {
                isError = false,
                code = 200,
                data = chuongTrinh,
                message = "Lấy chương trình thành công"
            };
        }
        catch
        {
            throw;
        }
    }
    
}
