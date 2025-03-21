using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
using StudyFlow.Application.LichHocs.Commands.EditLichHoc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class LichHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateLichHocCoDinh, "createlichhoccodinh")
            .MapPut(EditLichHoc, "editlichhoc")
            .MapGet(GetLichHocById, "getlichhocbyid");
    }
    [Authorize(Roles =Roles.CampusManager)]
    public async Task<Output> CreateLichHocCoDinh(ISender sender,CreateLichHocCommand command)
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
