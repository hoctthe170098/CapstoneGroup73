using Microsoft.AspNetCore.Mvc;
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
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay;
using StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemDanh;

namespace StudyFlow.Web.Endpoints;

public class DiemDanhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetDiemDanhTheoNgay, "getdiemdanhtheongay")
            .MapPost(UpdateDiemDanhTheoNgay,"updatediemdanhtheongay")
            .MapGet(GetBaoCaoDiemDanh,"getbaocaodiemdanh");
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetDiemDanhTheoNgay(ISender sender, [AsParameters] GetDiemDanhTheoNgayQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> UpdateDiemDanhTheoNgay(ISender sender, UpdateDiemDanhTheoNgayCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaoCaoDiemDanh(ISender sender, [AsParameters] GetBaoCaoDiemDanhQuery query)
    {
        return await sender.Send(query);
    }
}
