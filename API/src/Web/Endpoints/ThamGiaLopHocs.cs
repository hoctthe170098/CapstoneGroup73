using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Application.ThamGiaLopHocs.GetHocSinhsInClass;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class ThamGiaLopHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapPost(GetHocSinhsInClass, "gethocsinhsinclass");
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetHocSinhsInClass(ISender sender, GetHocSinhsInClassQueries query)
    {
        return await sender.Send(query);
    }
}
