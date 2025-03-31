using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using StudyFlow.Application.BaiTaps.Queries.TeacherAssignmentList;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class BaiTaps : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapGet(GetTeacherAssignmentList, "getteacherassignments");
    }

    [Authorize(Roles = Roles.Administrator + "," + Roles.Student)]
    public async Task<Output> GetTeacherAssignmentList(
        ISender sender,
        [AsParameters] TeacherAssignmentListWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
