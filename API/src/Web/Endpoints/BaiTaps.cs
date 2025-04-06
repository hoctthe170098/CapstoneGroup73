using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
using StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;
using StudyFlow.Application.BaiTaps.Commands.DeleteBaiTap;
using StudyFlow.Application.BaiTaps.Queries.TeacherAssignmentList;
using StudyFlow.Application.BaiTaps.Queries.GetAllBaiTaps;
using StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.ChinhSachs.Commands.DeleteBaiKiemTra;

namespace StudyFlow.Web.Endpoints;

public class BaiTaps : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapGet(GetTeacherAssignmentList, "getbaitapsforstudent")
           .MapGet(GetAllBaiTaps, "getallbaitaps")
           .MapGet(GetBaiTapsForTeacher, "getbaitapsforteacher")
           .MapPost(CreateBaiTap, "createbaitap")
           .MapPut(UpdateBaiTap, "updatebaitap")
           .MapDelete(DeleteBaiTap, "deletebaitap");
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetTeacherAssignmentList(
        ISender sender,
        [AsParameters] TeacherAssignmentListWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetAllBaiTaps(
        ISender sender,
        [AsParameters] GetAllBaiTapsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaiTapsForTeacher(ISender sender,[AsParameters] GetListBaiTapChoGiaoVienWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles =Roles.Teacher)]
    public async Task<Output> CreateBaiTap(ISender sender, [FromForm] CreateBaiTapCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles =Roles.Teacher)]
    public async Task<Output> UpdateBaiTap(ISender sender, [FromForm] UpdateBaiTapCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> DeleteBaiTap(ISender sender, string id)
    {
        try
        {
            Guid g_id = Guid.Parse(id);
            return await sender.Send(new DeleteBaiTapCommand(g_id));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
