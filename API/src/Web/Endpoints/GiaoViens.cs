using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;

namespace StudyFlow.Web.Endpoints;

public class GiaoViens : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateGiaoVien, "creategiaovien")
            .MapPost(GetGiaoViensWithPagination, "getgiaovienswithpagination")
            .MapPut(EditGiaoVien, "editgiaovien");
    }
    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> CreateGiaoVien(ISender sender, CreateGiaoVienCommand comand)
    {
        return await sender.Send(comand);
    }
    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetGiaoViensWithPagination(ISender sender, GetGiaoViensWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> EditGiaoVien(ISender sender, EditGiaoVienCommand comand)
    {
        return await sender.Send(comand);
    }
}
