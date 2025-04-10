using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using StudyFlow.Application.BaiTaps.Commands.CreateBaiTap;
using StudyFlow.Application.BaiTaps.Commands.DeleteBaiTap;
using StudyFlow.Application.BaiTaps.Commands.DownloadBaiTap;
using StudyFlow.Application.BaiTaps.Commands.UpdateBaiTap;
using StudyFlow.Application.BaiTaps.Queries.GetAllBaiTaps;
using StudyFlow.Application.BaiTaps.Queries.GetDetailBaiTapChoGiaoVien;
using StudyFlow.Application.BaiTaps.Queries.GetDetailBaiTapChoHocSinh;
using StudyFlow.Application.BaiTaps.Queries.GetListBaiTapChoGiaoVien;
using StudyFlow.Application.BaiTaps.Queries.TeacherAssignmentList;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StudyFlow.Web.Endpoints;

public class BaiTaps : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .DisableAntiforgery()
           .MapPost(GetTeacherAssignmentList, "getbaitapsforstudent")
           .MapPost(GetBaiTapsForTeacher, "getbaitapsforteacher")
           .MapPost(GetBaiTapDetailForTeacher, "getbaitapdetailforteacher")
           .MapPost(GetBaiTapDetailForStudent, "getbaitapdetailforstudent")
           .MapPost(DownloadBaiTapFile, "downloadbaitap")
           .MapPost(CreateBaiTap, "createbaitap")
           .MapPut(UpdateBaiTap, "updatebaitap")
           .MapDelete(DeleteBaiTap, "deletebaitap")
           .MapGet(GetAllBaiTaps, "getallbaitaps");
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetTeacherAssignmentList(
        ISender sender,
        [FromBody] TeacherAssignmentListWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaiTapsForTeacher(
        ISender sender,
        [FromBody] GetListBaiTapChoGiaoVienWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaiTapDetailForTeacher(
        ISender sender,
        [AsParameters] GetBaiTapDetailQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetBaiTapDetailForStudent(
    ISender sender,
        [AsParameters] GetDetailBaiTapChoHocSinhQuery query)
    {
        return await sender.Send(query);
    }

    [Authorize(Roles = Roles.Teacher + "," + Roles.Student)]
    public async Task<Output> DownloadBaiTapFile(
        ISender sender,
        [FromQuery] string filePath)
    {
        return await sender.Send(new DownloadBaiTapCommand { FilePath = filePath });
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> CreateBaiTap(
        ISender sender,
        [FromForm] CreateBaiTapCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> UpdateBaiTap(
        ISender sender,
        [FromForm] UpdateBaiTapCommand command)
    {
        return await sender.Send(command);
    }

    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> DeleteBaiTap(
        ISender sender,
        string id)
    {
        if (!Guid.TryParse(id, out var g_id))
            throw new ArgumentException("ID không hợp lệ");

        return await sender.Send(new DeleteBaiTapCommand(g_id));
    }

    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetAllBaiTaps(
        ISender sender,
        [AsParameters] GetAllBaiTapsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
