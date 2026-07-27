using AutoMapper;
using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Mappings;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<Contract, ContractDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));

        CreateMap<Contract, ContractDetailDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
            .ForMember(dest => dest.MilestoneCount, opt => opt.MapFrom(src => src.Milestones.Count))
            .ForMember(dest => dest.RiskFlagCount, opt => opt.MapFrom(src => src.RiskAnalyses.Count));

        // ClientId, Id, FreelancerId, DocumentPath, Status, timestamps are
        // all set explicitly in ContractService, not mapped from the DTO.
        CreateMap<CreateContractDto, Contract>();
        CreateMap<UpdateContractDto, Contract>();
    }
}