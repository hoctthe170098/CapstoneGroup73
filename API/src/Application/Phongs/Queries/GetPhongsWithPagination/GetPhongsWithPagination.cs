using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using System.Security.Claims;

namespace StudyFlow.Application.Phongs.Queries.GetPhongsWithPagination;

public record GetPhongsWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? CurrentRole { get; init; }
    public Guid? CurrentCoSoId { get; init; }  
}

public class GetPhongsWithPaginationQueryHandler : IRequestHandler<GetPhongsWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPhongsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetPhongsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");
            // Lấy danh sách phòng - mặc định cho Admin (xem tất cả)
            var query = _context.Phongs.AsNoTracking();
            // Nếu là Campus Manager, chỉ xem phòng trong CoSo của mình
            if (request.CurrentRole == Roles.CampusManager)
            {
                if (request.CurrentCoSoId == null)
                {
                    throw new Exception("Campus Manager không có thông tin cơ sở.");
                }

                query = query.Where(p => p.CoSoId == request.CurrentCoSoId);
            }

            var list = await query
                .ProjectTo<PhongDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return new Output
            {
                isError = false,
                data = list,
                code = 200
            };
        }
        catch (Exception ex)
        {
            throw new WrongInputException($"Có lỗi xảy ra khi lấy danh sách phòng: {ex.Message}");
        }
    }
}
