using AutoMapper;
using FreelanceFlow.Backend.DTOs.Clients;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public ClientService(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ClientDto>> GetAllForFreelancerAsync(Guid freelancerId)
    {
        var clients = await _clientRepository.GetByFreelancerIdAsync(freelancerId);
        return _mapper.Map<IReadOnlyList<ClientDto>>(clients);
    }

    public async Task<ServiceResult<ClientDto>> GetByIdAsync(Guid freelancerId, Guid clientId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);

        // Same "not found" message whether the client doesn't exist or just
        // belongs to a different freelancer — don't leak which case it is.
        if (client == null || client.FreelancerId != freelancerId)
        {
            return ServiceResult<ClientDto>.FailureResult("Client not found.");
        }

        return ServiceResult<ClientDto>.SuccessResult(_mapper.Map<ClientDto>(client));
    }

    public async Task<ServiceResult<ClientDto>> CreateAsync(Guid freelancerId, CreateClientDto dto)
    {
        var client = _mapper.Map<Client>(dto);
        client.Id = Guid.NewGuid();
        client.FreelancerId = freelancerId;
        client.IsDeleted = false;
        client.CreatedAt = DateTime.UtcNow;
        client.UpdatedAt = DateTime.UtcNow;

        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();

        return ServiceResult<ClientDto>.SuccessResult(_mapper.Map<ClientDto>(client));
    }

    public async Task<ServiceResult<ClientDto>> UpdateAsync(Guid freelancerId, Guid clientId, UpdateClientDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null || client.FreelancerId != freelancerId)
        {
            return ServiceResult<ClientDto>.FailureResult("Client not found.");
        }

        _mapper.Map(dto, client);
        client.UpdatedAt = DateTime.UtcNow;

        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();

        return ServiceResult<ClientDto>.SuccessResult(_mapper.Map<ClientDto>(client));
    }

    public async Task<ServiceResult<bool>> SoftDeleteAsync(Guid freelancerId, Guid clientId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null || client.FreelancerId != freelancerId)
        {
            return ServiceResult<bool>.FailureResult("Client not found.");
        }

        client.IsDeleted = true;
        client.UpdatedAt = DateTime.UtcNow;

        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }
}