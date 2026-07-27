using Microsoft.AspNetCore.Http;
using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface IContractService
{
    Task<IReadOnlyList<ContractDto>> GetAllForFreelancerAsync(Guid freelancerId);
    Task<ServiceResult<ContractDetailDto>> GetByIdAsync(Guid freelancerId, Guid contractId);
    Task<ServiceResult<ContractDto>> CreateAsync(Guid freelancerId, CreateContractDto dto);
    Task<ServiceResult<ContractDto>> UpdateAsync(Guid freelancerId, Guid contractId, UpdateContractDto dto);
    Task<ServiceResult<bool>> DeleteAsync(Guid freelancerId, Guid contractId);
    Task<ServiceResult<ContractDto>> UploadDocumentAsync(Guid freelancerId, Guid contractId, IFormFile file);
}