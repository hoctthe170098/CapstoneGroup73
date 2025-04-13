using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.KetQuaBaiKiemTras.Commands.UpdateKetQuaBaiKiemTra;
using StudyFlow.Application.KetQuaBaiKiemTras.Queries.GetDiemBaiKiemTraChoGiaoVien;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class KetQuaBaiKiemTras :EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetDiemBaiKiemTraChoGiaoVien, "getdiembaikiemtrachogiaovien")
            .MapPost(UpdateKetQuaBaiKiemTra,"updateketquabaikiemtra");
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> GetDiemBaiKiemTraChoGiaoVien(ISender sender, [AsParameters] GetDiemBaiKiemTraChoGiaoVienQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Teacher)]
    public async Task<Output> UpdateKetQuaBaiKiemTra(ISender sender, [FromForm] UpdateKetQuaBaiKiemTraCommand command)
    {
        return await sender.Send(command);
    }
}
