using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.Slots.Queries.GetSlots;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Slots.Commands;
using StudyFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace StudyFlow.Web.Endpoints;

public class Slots : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetSlots, "getslots") 
            .MapPost(CreateSlot, "createslot"); 
    }
    [Authorize(Roles = Roles.CampusManager)]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<List<SlotDto>> GetSlots(ISender sender)
    {
        return await sender.Send(new GetSlotsQuery());
    }
    [Authorize(Roles = Roles.CampusManager)]
    public async Task<Output> CreateSlot(ISender sender, [FromBody] CreateSlotCommand command)
    {
        return await sender.Send(command);
    }
}
