using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.TraLois.Commands.CreateTraLoi;
using StudyFlow.Application.TraLois.Commands.UpdateTraLoi;
using StudyFlow.Application.TraLois.Queries.GetBaiTapByTraLoi;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class TraLois : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapPost(CreateTraLoi, "create")
           .MapPut(UpdateTraLoi, "update")
           .MapGet(GetTraLoiByBaiTapId, "gettraloibybaitapid");
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> CreateTraLoi(ISender sender,[FromForm] CreateTraLoiCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> UpdateTraLoi(ISender sender,[FromForm] UpdateTraLoiCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Teacher + "," + Roles.Student)]
    public async Task<Output> GetTraLoiByBaiTapId(ISender sender,[AsParameters] GetTraLoiByBaiTapQuery query) 
    {
        return await sender.Send(query);
    }


}
