
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DashBoards.Queries.GetDashBoardChoAdmin;
using StudyFlow.Application.DashBoards.Queries.GetDashBoardChoCampusManager;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class DashBoards : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetDashBoardChoAdmin,"getdashboardadmin")
            .MapGet(GetDashBoardChoCampusManager,"getdashboardquanlycoso");
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetDashBoardChoAdmin(ISender sender, [AsParameters]GetDashBoardChoAdminQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetDashBoardChoCampusManager(ISender sender, [AsParameters] GetDashBoardChoCampusManagerQuery query)
    {
        return await sender.Send(query);
    }
}
