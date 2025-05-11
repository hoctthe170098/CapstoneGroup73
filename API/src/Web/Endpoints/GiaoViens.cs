using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.ChuongTrinhs.Commands.DowntaiLieuHocTap;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.DownloadTemplateExcelGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.ExportGiaoViensToExcel;
using StudyFlow.Application.GiaoViens.Commands.ImportGiaoViensFromExcel;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoVienAssignedClass;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensByNameOrCode;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class GiaoViens : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateGiaoVien, "creategiaovien")
            .MapPost(GetGiaoViensWithPagination, "getgiaovienswithpagination")
            .MapPost(GetGiaoViensByNameOrCode,"getgiaovienbycodeorname")
            .MapPut(EditGiaoVien, "editgiaovien")
            .MapPost(AddListGiaoViens, "addlistgiaoviens")
            .MapPost(ImportGiaoViensFromEXcel, "importgiaoviensfromexcel")
            .MapPost(ExportGiaoViensToExcel, "exportgiaovienstoexcel")
            .MapPost(GetGiaoVienAssignedClass, "getgiaovienassignedclass")
            .MapPost(DownloadTemplateExcelGiaoVien,"downloadtemplateexcelgiaovien");
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> CreateGiaoVien(ISender sender, CreateGiaoVienCommand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetGiaoViensWithPagination(ISender sender, GetGiaoViensWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetGiaoViensByNameOrCode(ISender sender, GetGiaoViensByNameOrCodeQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> EditGiaoVien(ISender sender, EditGiaoVienCommand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> AddListGiaoViens(ISender sender, AddListGiaoViensCommand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> ImportGiaoViensFromEXcel(ISender sender, [FromForm] ImportGiaoViensFromExcelCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<IResult> ExportGiaoViensToExcel(ISender sender, [FromBody] ExportGiaoViensToExcelCommand command)
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
    public async Task<IResult> DownloadTemplateExcelGiaoVien(ISender sender, [FromBody] DownloadTemplateExcelGiaoVienCommand command)
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
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetGiaoVienAssignedClass(ISender sender, GetGiaoVienAssignedClassWithPaginationCommand query)
    {
        return await sender.Send(query);
    }
}
