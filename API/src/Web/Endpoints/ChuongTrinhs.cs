using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class ChuongTrinhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateChuongTrinh, "createchuongtrinh")
            .MapGet(GetChuongTrinhsWithPagination, "getchuongtrinhs");
    }
    //[Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> CreateChuongTrinh(ISender sender, [FromBody] CreateChuongTrinhCommand command)
    {
        return await sender.Send(command);
    }
    //[Authorize(Roles = Roles.LearningManager)]
    public async Task<PaginatedList<ChuongTrinhDto>> GetChuongTrinhsWithPagination(ISender sender, [AsParameters] GetChuongTrinhsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
