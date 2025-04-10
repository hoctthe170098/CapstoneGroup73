using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.ThamGiaLopHocs.Queries.GetHocSinhAssignedClass;
using StudyFlow.Application.ThamGiaLopHocs.Queries.GetHocSinhsInClass;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class ThamGiaLopHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapPost(GetHocSinhsInClass, "gethocsinhsinclass")
           .MapPost(GetHocSinhAssignedClass, "gethocsinhassignedclass");
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetHocSinhsInClass(ISender sender, GetHocSinhsInClassQueries query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetHocSinhAssignedClass(ISender sender, GetHocSinhAssignedClassWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
