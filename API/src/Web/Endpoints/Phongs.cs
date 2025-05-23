﻿using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Phongs.Queries.GetPhongsWithPagination;
using StudyFlow.Application.Phongs.Commands.CreatePhong;
using StudyFlow.Application.Phongs.Commands.EditPhong;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.Phongs.Queries.GetAllPhongSuDungs;

namespace StudyFlow.Web.Endpoints;

public class Phongs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreatePhong, "createphong")
            .MapPost(GetPhongsWithPagination, "getphongswithpagination")
            .MapGet(GetAllPhongsSuDungDuoc,"getallphongsudungduoc")
            .MapPut(EditPhong, "editphong");
    }

    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> CreatePhong(ISender sender, CreatePhongCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetPhongsWithPagination(ISender sender, GetPhongsQuery query)
    {
        return await sender.Send(query);    
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetAllPhongsSuDungDuoc(ISender sender, [AsParameters]GetAllPhongSuDungsQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> EditPhong(ISender sender, EditPhongCommand command)
    {
        return await sender.Send(command);
    }
}
