using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.ApplicationUsers.Commands.Login;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Commands.CreateCoSo;
using StudyFlow.Application.Cosos.Commands.EditCoSo;
using StudyFlow.Application.Cosos.Queries.GetAllCoSo;
using StudyFlow.Application.Cosos.Queries.GetCososWithPagination;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class CoSos : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateCoSo,"createcoso")
            .MapPost(GetCoSosWithPagination, "getcososwithpagination")
            .MapGet(GetAllCoSos,"getallcosos")
            .MapPut(EditCoSo,"editcoso");
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> CreateCoSo(ISender sender, CreateCoSoComand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetCoSosWithPagination(ISender sender, GetCososWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetAllCoSos(ISender sender,[AsParameters] GetAllCoSosQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> EditCoSo(ISender sender, EditCoSoComand comand)
    {
        return await sender.Send(comand);
    }
}
