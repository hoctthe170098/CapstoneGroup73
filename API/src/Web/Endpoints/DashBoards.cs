
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.DashBoards.Queries.GetDashBoardChoAdmin;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class DashBoards : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).MapGet(GetDashBoardChoAdmin,"getdashboardadmin");
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetDashBoardChoAdmin(ISender sender, [AsParameters]GetDashBoardChoAdminQuery query)
    {
        return await sender.Send(query);
    }
}
