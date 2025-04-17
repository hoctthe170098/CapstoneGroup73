using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.TraLois.Commands.CreateTraLoi;
using StudyFlow.Application.TraLois.Commands.UpdateTraLoi;
using StudyFlow.Application.TraLois.Queries.GetBaiTapByTraLoi;
using StudyFlow.Application.TraLois.Queries.GetTraLoiByBaiTapForHocSinh;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.TraLois.Commands.DeleteTraLoi;
using StudyFlow.Application.TraLois.Commands.UpdateNhanXetTraLoi;
using StudyFlow.Application.TraLois.Queries.GetBaoCaoNhanXetHocSinh;

namespace StudyFlow.Web.Endpoints;

public class TraLois : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapPost(CreateTraLoi, "create")
           .MapPut(UpdateTraLoi, "update")
           .MapDelete(DeleteTraLoi, "delete")
           .MapPost(GetTraLoiByBaiTapIdForTeacher, "gettraloibybaitapforteacher") 
           .MapGet(GetTraLoiByBaiTapIdForStudent, "gettraloibybaitapforstudent")
           .MapPost(UpdateNhanXetTraLoi, "updatenhanxet")
           .MapGet(GetBaoCaoNhanXetHocSinh, "baocaonhanxet");
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> CreateTraLoi(ISender sender, [FromForm] CreateTraLoiCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> UpdateTraLoi(ISender sender, [FromForm] UpdateTraLoiCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetTraLoiByBaiTapIdForTeacher(ISender sender, [FromBody] GetTraLoiByBaiTapQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetTraLoiByBaiTapIdForStudent(ISender sender, [AsParameters] GetTraLoiByBaiTapForHocSinh query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Student)]
    public async Task<Output> DeleteTraLoi(ISender sender, [FromQuery] Guid traLoiId)
    {
        return await sender.Send(new DeleteTraLoiCommand(traLoiId));
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> UpdateNhanXetTraLoi(ISender sender, [FromBody] UpdateNhanXetTraLoiCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaoCaoNhanXetHocSinh(ISender sender, [FromQuery] string hocSinhCode)
    {
        return await sender.Send(new GetBaoCaoNhanXetHocSinhQuery { HocSinhCode = hocSinhCode });
    }
}
