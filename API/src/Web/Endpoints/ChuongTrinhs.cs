using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
using StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using Twilio.Rest.Microvisor.V1;

namespace StudyFlow.Web.Endpoints;

public class ChuongTrinhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateChuongTrinh, "createchuongtrinh")
            .MapPut(UpdateChuongTrinh,"updatechuongtrinh")
            .MapGet(GetChuongTrinhsWithPagination, "getchuongtrinhs");
    }
    //[Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> CreateChuongTrinh(ISender sender, [FromForm] CreateChuongTrinhCommand command)
    {
        return await sender.Send(command);
    }
    //[Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> UpdateChuongTrinh(ISender sender, [FromForm] UpdateChuongTrinhCommand command)
    {
        return await sender.Send(command);
    }
    //[Authorize(Roles = Roles.LearningManager)]
    public async Task<PaginatedList<ChuongTrinhDto>> GetChuongTrinhsWithPagination(ISender sender, [AsParameters] GetChuongTrinhsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
