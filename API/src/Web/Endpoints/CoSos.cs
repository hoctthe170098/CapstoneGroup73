using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.ApplicationUsers.Commands.Login;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Cosos.Commands.CreateCoSo;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class CoSos : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateCoSo, "CreateCoSo");
    }
    [Authorize(Policy = Policies.CanPurge)]
    public async Task<Output> CreateCoSo(ISender sender, CreateCoSoComand comand)
    {
        return await sender.Send(comand);
    }
}
