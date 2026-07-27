using AutoMapper;
using FreelanceFlow.Backend.DTOs.Clients;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Mappings;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, ClientDto>();

        // Create/Update DTOs only carry editable fields; Id, FreelancerId,
        // IsDeleted, CreatedAt/UpdatedAt are set explicitly in ClientService
        // rather than mapped, so they can't be spoofed by request payloads.
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();
    }
}