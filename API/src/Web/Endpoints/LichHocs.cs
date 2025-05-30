﻿using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.LichHocs.Commands.CreateLichHoc;
using StudyFlow.Application.LichHocs.Commands.EditLichHoc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.LichHocs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.LichHocs.Queries.GetLopHocWithPagination;
using StudyFlow.Application.LichHocs.Queries.GetLopHocByName;
using StudyFlow.Application.LichHocs.Queries.GetLopHocByTen;
using StudyFlow.Application.ChinhSachs.Commands.DeleteBaiKiemTra;
using StudyFlow.Application.ChinhSachs.Commands.DeleteLichHoc;
using StudyFlow.Application.LichHocs.Commands.CreateLichDayThay;
using StudyFlow.Application.LichHocs.Commands.UpdateLichDayThay;
using StudyFlow.Application.ChinhSachs.Commands.DeleteLichDayThay;
using StudyFlow.Application.LichHocs.Commands.CreateLichDayBu;
using StudyFlow.Application.LichHocs.Commands.UpdateLichDayBu;
using StudyFlow.Application.ChinhSachs.Commands.DeleteLichDayBu;
using StudyFlow.Application.LichHocs.Queries.GetLichHocGiaoVien;
using StudyFlow.Application.Cosos.Queries.GetDiemDanhTheoNgay;
using StudyFlow.Application.LichHocs.Queries.GetLichHocHocSinh;

namespace StudyFlow.Web.Endpoints;

public class LichHocs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetLopHocWithPagination, "getlophocwithpagination")
            .MapPost(CreateLichHocCoDinh, "createlichhoccodinh")
            .MapPut(EditLichHocCoDinh, "editlichhoccodinh")
            .MapGet(GetTenLopHocByName,"gettenlophocbyname")
            .MapGet(GetLopHocByTen,"getlophocbyten")
            .MapDelete(DeleteLichHocCoDinh,"deletelophoccodinh")
            .MapPost(CreateLichHocDayThay,"createlichdaythay")
            .MapPut(UpdateLichHocDayThay,"updatelichdaythay")
            .MapDelete(DeleteLichHocDayThay,"deletelichdaythay")
            .MapPost(CreateLichHocDayBu,"createlichdaybu")
            .MapPut(UpdateLichHocDayBu, "updatelichdaybu")
            .MapDelete(DeleteLichHocDayBu, "deletelichdaybu")
            .MapGet(GetLichHocGiaoVien,"getlichhocgiaovien")
            .MapGet(GetLichHocHocSinh,"getlichhochocsinh");
    }
    [Authorize(Roles =Roles.CampusManager)]
    public async Task<Output> CreateLichHocCoDinh(ISender sender, [FromBody] CreateLichHocCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> EditLichHocCoDinh(ISender sender, [FromBody] EditLichHocCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetLopHocWithPagination(ISender sender, GetLopHocWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager+","+Roles.LearningManager)]
    public async Task<Output> GetTenLopHocByName(ISender sender, [AsParameters]GetLopHocByNameQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetLopHocByTen(ISender sender, [AsParameters] GetLopHocByTenQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> DeleteLichHocCoDinh(ISender sender, string tenLopHoc)
    {
        return await sender.Send(new DeleteLichHocCommand(tenLopHoc));
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> CreateLichHocDayThay(ISender sender,  CreateLichDayThayCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> UpdateLichHocDayThay(ISender sender, UpdateLichDayThayCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> DeleteLichHocDayThay(ISender sender, Guid lichHocId)
    {
        return await sender.Send(new DeleteLichDayThayCommand(lichHocId));
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> CreateLichHocDayBu(ISender sender, CreateLichDayBuCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> UpdateLichHocDayBu(ISender sender, UpdateLichDayBuCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> DeleteLichHocDayBu(ISender sender, Guid lichHocId)
    {
        return await sender.Send(new DeleteLichDayBuCommand(lichHocId));
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetLichHocGiaoVien(ISender sender, [AsParameters] GetLichHocGiaoVienQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetLichHocHocSinh(ISender sender, [AsParameters] GetLichHocHocSinhQuery query)
    {
        return await sender.Send(query);
    }
}
