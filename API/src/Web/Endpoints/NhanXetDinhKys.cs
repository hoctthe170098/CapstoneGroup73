
using System;
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.ChinhSachs.Commands.DeleteBaiKiemTra;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanXetDinhKys.Commands.CreateNhanXetDinhKy;
using StudyFlow.Application.NhanXetDinhKys.Commands.DeleteNhanXetDinhKy;
using StudyFlow.Application.NhanXetDinhKys.Commands.UpdateNhanXetDinhKy;
using StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class NhanXetDinhKys : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetNhanXetDinhKy, "getnhanxetdinhky")
            .MapPost(CreateNhanXetDinhKy, "createnhanxetdinhky")
            .MapPut(UpdateNhanXetDinhKy, "updatenhanxetdinhky")
            .MapDelete(DeleteNhanXetDinhKy,"deletenhanxetdinhky");
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetNhanXetDinhKy(ISender sender, [AsParameters] GetNhanXetDinhKysQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> CreateNhanXetDinhKy(ISender sender, CreateNhanXetDinhKyCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> UpdateNhanXetDinhKy(ISender sender, UpdateNhanXetDinhKyCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> DeleteNhanXetDinhKy(ISender sender, Guid id)
    {
        return await sender.Send(new DeleteNhanXetDinhKyCommand(id));
    }
}
