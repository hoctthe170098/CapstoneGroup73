using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanViens.Queries.GetNhanViensWithPagination;
using StudyFlow.Application.NhanViens.Command.CreateNhanVien;
using StudyFlow.Domain.Constants;
using StudyFlow.Application.NhanViens.Command.EditNhanVien;

namespace StudyFlow.Web.Endpoints;

public class NhanViens : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateNhanVien, "createnhanvien")
            .MapPost(GetNhanViensWithPagination, "getnhanvienswithpagination")
            .MapPut(EditNhanVien, "editnhanvien");
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> CreateNhanVien(ISender sender, CreateNhanVienCommand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetNhanViensWithPagination(ISender sender, GetNhanViensWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    [Authorize(Roles = Roles.Administrator)]
    public async Task<Output> EditNhanVien(ISender sender, EditNhanVienCommand comand)
    {
        return await sender.Send(comand);
    }
}
