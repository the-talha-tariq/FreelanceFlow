using AutoMapper;
using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Mappings;

public class ContractRiskAnalysisProfile : Profile
{
    public ContractRiskAnalysisProfile()
    {
        CreateMap<ContractRiskAnalysis, RiskAnalysisDto>();
    }
}