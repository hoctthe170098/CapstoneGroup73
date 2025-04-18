
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanXetDinhKys.Commands.CreateNhanXetDinhKy;
using StudyFlow.Application.NhanXetDinhKys.Queries.GetNhanXetDinhKy;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class NhanXetDinhKys : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetNhanXetDinhKy,"getnhanxetdinhky")
            .MapPost(CreateNhanXetDinhKy,"createnhanxetdinhky");
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetNhanXetDinhKy(ISender sender,[AsParameters] GetNhanXetDinhKysQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> CreateNhanXetDinhKy(ISender sender, CreateNhanXetDinhKyCommand command)
    {
        return await sender.Send(command);
    }
}
