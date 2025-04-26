using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.ExportGiaoViensToExcel;
using StudyFlow.Application.KetQuaBaiKiemTras.Commands.ExportKetQuaBaiKiemTraToExcel;
using StudyFlow.Application.KetQuaBaiKiemTras.Commands.UpdateKetQuaBaiKiemTra;
using StudyFlow.Application.KetQuaBaiKiemTras.Queries.GetDiemBaiKiemTraChoGiaoVien;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class KetQuaBaiKiemTras :EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetDiemBaiKiemTraChoGiaoVien, "getdiembaikiemtrachogiaovien")
            .MapPost(UpdateKetQuaBaiKiemTra,"updateketquabaikiemtra")
            .MapPost(ExportKetQuaBaiKiemTraToExcel,"exportketquabaikiemtra");
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetDiemBaiKiemTraChoGiaoVien(ISender sender, [AsParameters] GetDiemBaiKiemTraChoGiaoVienQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> UpdateKetQuaBaiKiemTra(ISender sender, [FromBody] UpdateKetQuaBaiKiemTraCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<IResult> ExportKetQuaBaiKiemTraToExcel(ISender sender, [FromBody] ExportKetQuaBaiKiemTraToExcelCommand command)
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
}
