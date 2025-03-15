using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Mappings;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ChinhSachs.Queries.GetChinhSachsWithPagination;

public record GetChinhSachsWithPaginationQuery : IRequest<Output>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetChinhSachsWithPaginationQueryHandler : IRequestHandler<GetChinhSachsWithPaginationQuery, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetChinhSachsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Output> Handle(GetChinhSachsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
            throw new WrongInputException("Số trang hoặc kích thước trang không hợp lệ!");

        var list = await _context.ChinhSaches
            .AsNoTracking()
            .ProjectTo<ChinhSachDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return new Output
        {
            isError = false,
            data = list,
            code = 200,
            message = "Lấy danh sách chính sách thành công"
        };
    }
}
