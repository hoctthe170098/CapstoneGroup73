using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Web.Endpoints;

public class ChuongTrinhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateChuongTrinh, "chuongtrinh/create")
            .MapGet(GetChuongTrinhsWithPagination, "chuongtrinh/list");
    }

    public async Task<Output> CreateChuongTrinh(ISender sender, [FromBody] CreateChuongTrinhCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<PaginatedList<ChuongTrinhDto>> GetChuongTrinhsWithPagination(ISender sender, [AsParameters] GetChuongTrinhsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
