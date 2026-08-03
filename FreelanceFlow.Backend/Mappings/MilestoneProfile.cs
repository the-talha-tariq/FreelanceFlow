using AutoMapper;
using FreelanceFlow.Backend.DTOs.Milestones;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Mappings;

public class MilestoneProfile : Profile
{
    public MilestoneProfile()
    {
        CreateMap<Milestone, MilestoneDto>()
            .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Id : (Guid?)null));

        // Id, ContractId, Status, CompletedAt, timestamps are all set
        // explicitly in MilestoneService, not mapped from the DTO.
        CreateMap<CreateMilestoneDto, Milestone>();
        CreateMap<UpdateMilestoneDto, Milestone>();
    }
}