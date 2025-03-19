using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.ExportGiaoViensToExcel;
using StudyFlow.Application.GiaoViens.Commands.ImportGiaoViensFromExcel;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensByNameOrCode;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class GiaoViens : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateGiaoVien, "creategiaovien")
            .MapPost(GetGiaoViensWithPagination, "getgiaovienswithpagination")
            .MapPost(GetGiaoViensByNameOrCode,"getgiaovienbycodeorname")
            .MapPut(EditGiaoVien, "editgiaovien")
            .MapPost(AddListGiaoViens, "addlistgiaoviens")
            .MapPost(ImportGiaoViensFromEXcel, "importgiaoviensfromexcel")
            .MapPost(ExportGiaoViensToExcel, "exportgiaovienstoexcel");
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
    //[Authorize(Roles = Roles.CampusManager)]
    [Consumes("multipart/form-data")]
    public async Task<Output> ImportGiaoViensFromEXcel(ISender sender, [FromForm] ImportGiaoViensFromExcelCommand command)
    {
        return await sender.Send(command);
    }
    //[Authorize(Roles = Roles.CampusManager)]
    public async Task<FileContentResult> ExportGiaoViensToExcel(ISender sender, ExportGiaoViensToExcelCommand comand)
    {
        return await sender.Send(comand);
    }
}
