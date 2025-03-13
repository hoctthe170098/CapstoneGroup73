using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;
namespace StudyFlow.Application.Phongs.Commands.CreatePhong;

public record CreatePhongCommand : IRequest<Output>
{
    public required string Ten { get; init; }
}

public class CreatePhongCommandHandler : IRequestHandler<CreatePhongCommand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreatePhongCommandHandler(IApplicationDbContext context, IIdentityService identityService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Output> Handle(CreatePhongCommand request, CancellationToken cancellationToken)
    {
        // Validate đầu vào
        ValidateRequest(request);

        // Lấy token từ request header
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc bị thiếu.");
        var coSoId = _identityService.GetCampusId(token);
        // Kiểm tra CoSo tồn tại
        var coSoExists = await _context.CoSos.AnyAsync(cs => cs.Id == coSoId, cancellationToken);
        if (!coSoExists)
            throw new NotFoundDataException("Không tìm thấy cơ sở.");
       
        // Check trùng tên phòng trong cơ sở
        var isPhongExisted = await _context.Phongs
            .AnyAsync(p => p.Ten.Trim().ToLower() == request.Ten.Trim().ToLower() && p.CoSoId == coSoId, cancellationToken);

        if (isPhongExisted)
            throw new Exception($"Tên phòng '{request.Ten}' đã tồn tại trong cơ sở này.");

        // Tạo mới phòng với trạng thái mặc định là 'use'
        var phong = new Phong
        {
            Ten = request.Ten.Trim(),
            TrangThai = "use", // Trạng thái mặc định
            CoSoId = coSoId
        };

        _context.Phongs.Add(phong);
        await _context.SaveChangesAsync(cancellationToken);

        return new Output
        {
            isError = false,
            data = phong,
            code = 200,
            message = "Tạo phòng thành công"
        };
    }

    private void ValidateRequest(CreatePhongCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Ten))
            throw new WrongInputException("Tên phòng không được để trống!");

        if (request.Ten.Length > 20)
            throw new WrongInputException("Tên phòng không được dài quá 20 ký tự!");
        
    }
}
