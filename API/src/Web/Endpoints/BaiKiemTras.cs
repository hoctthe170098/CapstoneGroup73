using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.BaiKiemTras.Queries.GetBaiKiemTrasWithPagination;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class BaiKiemTras : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetBaiKiemTrasWithPagination, "getbaikiemtraswithpagination");
    }

    [Authorize(Roles = Roles.LearningManager)]
    public async Task<Output> GetBaiKiemTrasWithPagination(ISender sender,GetBaiKiemTrasWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
}
