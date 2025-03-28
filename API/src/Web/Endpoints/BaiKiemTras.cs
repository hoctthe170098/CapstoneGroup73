using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.ChinhSachs.Commands.DeleteBaiKiemTra;
using StudyFlow.Application.ChinhSachs.Commands.DeleteChinhSach;
using StudyFlow.Application.ChuongTrinhs.Commands.CreateBaiKiemTra;
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
            .MapDelete(DeleteBaiKiemTra,"deletebaikiemtra");
    }

    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> GetBaiKiemTrasWithPagination(ISender sender,GetBaiKiemTrasWithPaginationQuery query)
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
}
