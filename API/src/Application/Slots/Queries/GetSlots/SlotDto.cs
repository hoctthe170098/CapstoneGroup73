using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.Slots.Queries.GetSlots;

public class SlotDto
{
    public int Id { get; init; }
    public string Ten { get; init; } = string.Empty;
    public TimeOnly BatDau { get; init; }
    public TimeOnly KetThuc { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Slot, SlotDto>();
        }
    }
}
