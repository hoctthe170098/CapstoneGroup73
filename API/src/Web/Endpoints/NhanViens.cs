using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Commands.CreateCoSo;
using StudyFlow.Application.Cosos.Commands.EditCoSo;
using StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class NhanViens : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateCoSo, "createcoso")
            .MapPost(GetNhanViensWithPagination, "getnhanvienswithpagination")
            .MapPut(EditCoSo, "editcoso");
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> CreateCoSo(ISender sender, CreateCoSoComand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetNhanViensWithPagination(ISender sender, GetNhanViensWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> EditCoSo(ISender sender, EditCoSoComand comand)
    {
        return await sender.Send(comand);
    }
}
