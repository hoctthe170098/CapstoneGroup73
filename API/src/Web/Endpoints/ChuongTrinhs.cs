using CleanArchitecture.Application.ChuongTrinhs.Commands.DeleteChuongTrinh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.ChuongTrinhs.Commands.CreateChuongTrinh;
using StudyFlow.Application.ChuongTrinhs.Commands.DowntaiLieuHocTap;
using StudyFlow.Application.ChuongTrinhs.Commands.UpdateChuongTrinh;
using StudyFlow.Application.ChuongTrinhs.Queries.GetAllChuongTrinhs;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhById;
using StudyFlow.Application.ChuongTrinhs.Queries.GetChuongTrinhsWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Queries.GetAllCoSo;
using StudyFlow.Domain.Constants;
using Twilio.Rest.Microvisor.V1;
namespace StudyFlow.Web.Endpoints;
public class ChuongTrinhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(CreateChuongTrinh, "createchuongtrinh")
            .MapPut(UpdateChuongTrinh,"updatechuongtrinh")
            .MapPost(GetChuongTrinhsWithPagination, "getchuongtrinhs")
            .MapGet(GetAllChuongTrinhs, "getallchuongtrinhs")
            .MapGet(GetChuongTrinhById, "getchuongtrinhbyid/{chuongTrinhId}")
            .MapDelete(DeleteChuongTrinh,"deletechuongtrinh/{id}")
            .MapPost(DownloadTaiLieuHocTap, "downloadtailieuhoctap"); ;
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> CreateChuongTrinh(ISender sender, [FromForm] CreateChuongTrinhCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> UpdateChuongTrinh(ISender sender, [FromForm] UpdateChuongTrinhCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize]
    public async Task<IResult> DownloadTaiLieuHocTap(ISender sender, [FromBody] DowntaiLieuHocTapCommand command)
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
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> GetChuongTrinhsWithPagination(ISender sender, GetChuongTrinhsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> GetChuongTrinhById(ISender sender,[AsParameters] GetChuongTrinhByIdQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> GetAllChuongTrinhs(ISender sender, [AsParameters] GetAllChuongTrinhsQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> DeleteChuongTrinh(ISender sender, string id)
    {
        try
        {
            int i_id = Int32.Parse(id);
            return await sender.Send(new DeleteChuongTrinhComand(i_id));
        }catch (Exception)
        {
            throw;
        }
    }
}
