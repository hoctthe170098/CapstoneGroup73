using Microsoft.AspNetCore.Mvc;
using StudyFlow.Application.Slots.Queries.GetSlots;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.Slots.Commands;

namespace StudyFlow.Web.Endpoints;

public class Slots : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetSlots, "slots") 
            .MapPost(CreateSlot, "slots/create"); 
    }

    public async Task<List<SlotDto>> GetSlots(ISender sender)
    {
        return await sender.Send(new GetSlotsQuery());
    }

    public async Task<Output> CreateSlot(ISender sender, [FromBody] CreateSlotCommand command)
    {
        return await sender.Send(command);
    }
}
