using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
using StudyFlow.Application.LichHocs.Commands.EditLichHoc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Application.LichHocs.Queries.GetLopHocByName;

namespace StudyFlow.Web.Endpoints;

public class LichHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetLopHocWithPagination, "getlophocwithpagination")
            .MapPost(CreateLichHocCoDinh, "createlichhoccodinh")
            .MapPut(EditLichHoc, "editlichhoc")
            .MapGet(GetTenLopHocByName,"gettenlophocbyname");
    }
    [Authorize(Roles =Roles.CampusManager)]
    public async Task<Output> CreateLichHocCoDinh(ISender sender, [FromBody] CreateLichHocCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<Output> EditLichHoc(ISender sender, [FromBody] EditLichHocCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetLopHocWithPagination(ISender sender, GetLopHocWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager+","+Roles.LearningManager)]
    public async Task<Output> GetTenLopHocByName(ISender sender, [AsParameters]GetLopHocByNameQuery query)
    {
        return await sender.Send(query);
    }
}
