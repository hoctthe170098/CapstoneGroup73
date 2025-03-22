using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.ExportGiaoViensToExcel;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;
using StudyFlow.Application.HocSinhs.Commands.AddListHocSinhs;
using StudyFlow.Application.HocSinhs.Commands.CreateHocSinh;
using StudyFlow.Application.HocSinhs.Commands.EditHocSinh;
using StudyFlow.Application.HocSinhs.Commands.ExportHocSinhsToExcel;
using StudyFlow.Application.HocSinhs.Commands.ImportHocSinhFromExcel;
using StudyFlow.Application.HocSinhs.Queries.GetHocViensWithPagination;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class HocSinhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateHocSinh, "createhocsinh")
            .MapPost(GetHocSinhsWithPagination, "gethocsinhswithpagination")
            .MapPut(EditHocSinh, "edithocsinh")
            .MapPost(AddListHocSinhs, "addlisthocsinhs")
            .MapPost(ExportHocSinhsToExcel, "exporthocsinhstoexcel")
            .MapPost(ImportHocSinhsFromExcel, "importhocsinhsfromexcel");
    }

    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> CreateHocSinh(ISender sender, CreateHocSinhCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetHocSinhsWithPagination(ISender sender, GetHocSinhsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> EditHocSinh(ISender sender, EditHocSinhCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> AddListHocSinhs(ISender sender, AddListHocSinhsCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<IResult> ExportHocSinhsToExcel(ISender sender, [FromBody] ExportHocSinhsToExcelCommand command)
    {
        var result = await sender.Send(command);

        if (result.isError == true)
        {
            return Results.BadRequest(result.message);
        }

        if (result.data == null)
        {
            return Results.BadRequest("File not found or error occurred.");
        }

        if (!(result.data is FileContentResult fileContentResult))
        {
            return Results.BadRequest("Invalid file content.");
        }

        return Results.File(fileContentResult.FileContents, fileContentResult.ContentType, fileContentResult.FileDownloadName);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> ImportHocSinhsFromExcel(ISender sender, [FromForm] ImportHocSinhFromExcelCommand command)
    {
        return await sender.Send(command);
    }
}

