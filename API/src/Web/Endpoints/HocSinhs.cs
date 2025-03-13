using StudyFlow.Application.Common.Models;
using StudyFlow.Application.GiaoViens.Commands.AddListGiaoViens;
using StudyFlow.Application.GiaoViens.Commands.CreateGiaoVien;
using StudyFlow.Application.GiaoViens.Commands.EditGiaoVien;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoViensWithPagination;
using StudyFlow.Application.HocSinhs.Commands.AddListHocSinhs;
using StudyFlow.Application.HocSinhs.Commands.CreateHocSinh;
using StudyFlow.Application.HocSinhs.Commands.EditHocSinh;
using StudyFlow.Application.HocSinhs.Queries.GetHocViensWithPagination;

namespace StudyFlow.Web.Endpoints;

public class HocSinhs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateHocSinh, "createhocsinh")
            .MapPost(GetHocSinhsWithPagination, "gethocsinhswithpagination")
            .MapPut(EditHocSinh, "edithocsinh")
            .MapPost(AddListHocSinhs, "addlisthocsinhs");
    }

    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> CreateHocSinh(ISender sender, CreateHocSinhCommand command)
    {
        return await sender.Send(command);
    }

    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> GetHocSinhsWithPagination(ISender sender, GetHocSinhsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> EditHocSinh(ISender sender, EditHocSinhCommand command)
    {
        return await sender.Send(command);
    }

    //[Authorize(Roles = Roles.Administrator)]
    public async Task<Output> AddListHocSinhs(ISender sender, AddListHocSinhsCommand command)
    {
        return await sender.Send(command);
    }
}

