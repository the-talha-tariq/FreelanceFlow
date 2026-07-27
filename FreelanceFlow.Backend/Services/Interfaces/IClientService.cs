using FreelanceFlow.Backend.DTOs.Clients;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface IClientService
{
    Task<IReadOnlyList<ClientDto>> GetAllForFreelancerAsync(Guid freelancerId);
    Task<ServiceResult<ClientDto>> GetByIdAsync(Guid freelancerId, Guid clientId);
    Task<ServiceResult<ClientDto>> CreateAsync(Guid freelancerId, CreateClientDto dto);
    Task<ServiceResult<ClientDto>> UpdateAsync(Guid freelancerId, Guid clientId, UpdateClientDto dto);
    Task<ServiceResult<bool>> SoftDeleteAsync(Guid freelancerId, Guid clientId);
}