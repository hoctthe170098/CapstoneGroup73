using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.BaiKiemTras.Queries.GetDiemBaiKiemTraChoGiaoVien;
using StudyFlow.Application.BaiKiemTras.Queries.GetLichKiemTraChoGiaoVien;
using StudyFlow.Application.ChinhSachs.Commands.DeleteBaiKiemTra;
using StudyFlow.Application.ChinhSachs.Commands.DeleteChinhSach;
using StudyFlow.Application.ChuongTrinhs.Commands.CreateBaiKiemTra;
using StudyFlow.Application.ChuongTrinhs.Commands.DownBaiThi;
using StudyFlow.Application.ChuongTrinhs.Commands.DowntaiLieuHocTap;
using StudyFlow.Application.ChuongTrinhs.Commands.UpdateBaiKiemTra;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class BaiKiemTras : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(GetBaiKiemTrasWithPagination, "getbaikiemtraswithpagination")
            .MapPost(CreateBaiKiemTra,"createbaikiemtra")
            .MapPut(UpdateBaiKiemTra,"updatebaikiemtra")
            .MapDelete(DeleteBaiKiemTra,"deletebaikiemtra")
            .MapPost(DownloadBaiKiemTra, "downloadbaikiemtra")
            .MapGet(GetLichKiemTraChoGiaoVien,"getlichkiemtrachogiaovien")
            .MapGet(GetDiemBaiKiemTraChoGiaoVien,"getdiembaikiemtrachogiaovien");
    }

    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> GetBaiKiemTrasWithPagination(ISender sender,GetBaiKiemTrasWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetLichKiemTraChoGiaoVien(ISender sender,[AsParameters] GetLichKiemTraChoGiaoVienQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetDiemBaiKiemTraChoGiaoVien(ISender sender, [AsParameters] GetDiemBaiKiemTraChoGiaoVienQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> CreateBaiKiemTra(ISender sender, [FromForm] CreateBaiKiemTraCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> UpdateBaiKiemTra(ISender sender, [FromForm] UpdateBaiKiemTraCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> DeleteBaiKiemTra(ISender sender, string id)
    {
        try
        {
            Guid g_id = Guid.Parse(id);
            return await sender.Send(new DeleteBaiKiemTraCommand(g_id));
        }
        catch (Exception)
        {
            throw;
        }
    }
    [Authorize(Roles=Roles.LearningManager+","+Roles.Teacher)]
    public async Task<IResult> DownloadBaiKiemTra(ISender sender, [FromBody] DownBaiThiCommand command)
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
