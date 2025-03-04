using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
using StudyFlow.Application.LichHocs.Commands.EditLichHoc;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Web.Endpoints;

public class LichHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateLichHoc, "lichhoc/create")
            .MapPut(EditLichHoc, "lichhoc/edit");
    }

    public async Task<Output> CreateLichHoc(ISender sender, [FromBody] CreateLichHocCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<Output> EditLichHoc(ISender sender, [FromBody] EditLichHocCommand command)
    {
        return await sender.Send(command);
    }
}
