using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.ChinhSachs.Commands.CreateChinhSach;
using StudyFlow.Application.ChinhSachs.Commands.UpdateChinhSach;
using StudyFlow.Application.ChinhSachs.Commands.DeleteChinhSach;
using StudyFlow.Application.ChinhSachs.Queries.GetAllChinhSachs;
using StudyFlow.Application.ChinhSachs.Queries.GetChinhSachsWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.ChinhSaches.Commands.CreateChinhSach;

namespace StudyFlow.Web.Endpoints;

public class ChinhSachs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateChinhSach, "createchinhsach")
            .MapPut(UpdateChinhSach, "updatechinhsach")
            .MapPost(GetChinhSachsWithPagination, "getchinhsachs")
            .MapGet(GetAllChinhSachs, "getallchinhsachs")
            .MapDelete(DeleteChinhSach, "deletechinhsach/{id}");
    }

    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> CreateChinhSach(ISender sender, [FromBody] CreateChinhSachCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> UpdateChinhSach(ISender sender, [FromBody] UpdateChinhSachCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetChinhSachsWithPagination(ISender sender, GetChinhSachsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetAllChinhSachs(ISender sender, [AsParameters] GetAllChinhSachsQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> DeleteChinhSach(ISender sender, string id)
    {
        try
        {
            int i_id = Int32.Parse(id);
            return await sender.Send(new DeleteChinhSachCommand(i_id));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
