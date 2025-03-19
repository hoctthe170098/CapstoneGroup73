using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
using StudyFlow.Application.LichHocs.Commands.EditLichHoc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries;
using MediatR;

namespace StudyFlow.Web.Endpoints;

public class LichHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateLichHoc, "createlichhoc")
            .MapPut(EditLichHoc, "editlichhoc")
            .MapGet(GetLichHocById, "getlichhocbyid");
    }

    public async Task<Output> CreateLichHoc(ISender sender, CreateLichHocCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<Output> EditLichHoc(ISender sender, [FromBody] EditLichHocCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<Output> GetLichHocById(ISender sender, Guid id)
    {
        return await sender.Send(new GetLichHocByIdQuery(id));
    }
}
