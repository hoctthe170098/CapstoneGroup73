using StudyFlow.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.Cosos.Queries.GetDiemDanhTheoNgay;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanhTheoNgay;
using StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemDanh;
using StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemHangNgayCuaHocSinh;
using StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemDanhHocSinh;
using StudyFlow.Application.Cosos.Queries.GetBaoCaoDiemDanhChoTungLop;
using StudyFlow.Application.DiemDanhs.Commands.UpdateDiemDanh;
using StudyFlow.Application.Cosos.Queries.GetBaoCaoHocPhiChoTungLop;

namespace StudyFlow.Web.Endpoints;

public class DiemDanhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetDiemDanhTheoNgay, "getdiemdanhtheongay")
            .MapPost(UpdateDiemDanhTheoNgay,"updatediemdanhtheongay")
            .MapGet(GetBaoCaoDiemDanh,"getbaocaodiemdanh")
            .MapGet(GetBaoCaoDiemHangNgayCuaHocSinh,"getbaocaodiemhangngaycuahocsinh")
            .MapGet(GetBaoCaoDiemDanhHocSinh,"getbaocaodiemdanhhocsinh")
            .MapGet(GetBaoCaoDiemDanhChoTungLop,"getbaocaodiemdanhchotunglop")
            .MapPost(UpdateDiemDanh,"updatediemdanh")
            .MapGet(GetBaoCaoHocPhiChoTungLop,"getbaocaohocphi");
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
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> UpdateDiemDanh(ISender sender, UpdateDiemDanhCommand command)
    {
        return await sender.Send(command);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaoCaoDiemDanh(ISender sender, [AsParameters] GetBaoCaoDiemDanhQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetBaoCaoDiemDanhChoTungLop(ISender sender, [AsParameters] GetBaoCaoDiemDanhChoTungLopQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> GetBaoCaoHocPhiChoTungLop(ISender sender, [AsParameters] GetBaoCaoHocPhiChoTungLopQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetBaoCaoDiemHangNgayCuaHocSinh(ISender sender, [AsParameters] GetBaoCaoDiemHangNgayCuaHocSinhQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Student)]
    public async Task<Output> GetBaoCaoDiemDanhHocSinh(ISender sender, [AsParameters] GetBaoCaoDiemDanhHocSinhQuery query)
    {
        return await sender.Send(query);
    }
}
