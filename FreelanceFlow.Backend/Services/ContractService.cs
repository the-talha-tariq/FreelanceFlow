using AutoMapper;
using Microsoft.AspNetCore.Http;
using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class ContractService : IContractService
{
    private static readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt" };
    private const long MaxDocumentSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IContractRepository _contractRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public ContractService(
        IContractRepository contractRepository,
        IClientRepository clientRepository,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _contractRepository = contractRepository;
        _clientRepository = clientRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ContractDto>> GetAllForFreelancerAsync(Guid freelancerId)
    {
        var contracts = await _contractRepository.GetByFreelancerIdAsync(freelancerId);
        return _mapper.Map<IReadOnlyList<ContractDto>>(contracts);
    }

    public async Task<ServiceResult<ContractDetailDto>> GetByIdAsync(Guid freelancerId, Guid contractId)
    {
        var contract = await _contractRepository.GetWithDetailsAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<ContractDetailDto>.FailureResult("Contract not found.");
        }

        return ServiceResult<ContractDetailDto>.SuccessResult(_mapper.Map<ContractDetailDto>(contract));
    }

    public async Task<ServiceResult<ContractDto>> CreateAsync(Guid freelancerId, CreateContractDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (client == null || client.FreelancerId != freelancerId)
        {
            return ServiceResult<ContractDto>.FailureResult("Client not found.");
        }

        var contract = _mapper.Map<Contract>(dto);
        contract.Id = Guid.NewGuid();
        contract.FreelancerId = freelancerId;
        contract.CreatedAt = DateTime.UtcNow;
        contract.UpdatedAt = DateTime.UtcNow;

        await _contractRepository.AddAsync(contract);
        await _contractRepository.SaveChangesAsync();

        // Assign the already-fetched Client rather than re-querying, so
        // ContractDto.ClientName maps without a second round trip.
        contract.Client = client;
        return ServiceResult<ContractDto>.SuccessResult(_mapper.Map<ContractDto>(contract));
    }

    public async Task<ServiceResult<ContractDto>> UpdateAsync(Guid freelancerId, Guid contractId, UpdateContractDto dto)
    {
        // GetWithDetailsAsync (not the plain GetByIdAsync) so Client is
        // loaded and ContractDto.ClientName maps correctly below.
        var contract = await _contractRepository.GetWithDetailsAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<ContractDto>.FailureResult("Contract not found.");
        }

        _mapper.Map(dto, contract);
        contract.UpdatedAt = DateTime.UtcNow;

        _contractRepository.Update(contract);
        await _contractRepository.SaveChangesAsync();

        return ServiceResult<ContractDto>.SuccessResult(_mapper.Map<ContractDto>(contract));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid freelancerId, Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<bool>.FailureResult("Contract not found.");
        }

        if (!string.IsNullOrEmpty(contract.DocumentPath))
        {
            _fileStorageService.DeleteFile(contract.DocumentPath);
        }

        // Milestones and risk analyses cascade-delete via the FK
        // configuration in ContractConfiguration.
        _contractRepository.Remove(contract);
        await _contractRepository.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<ContractDto>> UploadDocumentAsync(Guid freelancerId, Guid contractId, IFormFile file)
    {
        // GetWithDetailsAsync so Client is loaded for the ContractDto mapping below.
        var contract = await _contractRepository.GetWithDetailsAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<ContractDto>.FailureResult("Contract not found.");
        }

        if (file.Length == 0)
        {
            return ServiceResult<ContractDto>.FailureResult("The uploaded file is empty.");
        }

        if (file.Length > MaxDocumentSizeBytes)
        {
            return ServiceResult<ContractDto>.FailureResult("The uploaded file exceeds the 10 MB limit.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedDocumentExtensions.Contains(extension))
        {
            return ServiceResult<ContractDto>.FailureResult(
                $"Unsupported file type. Allowed types: {string.Join(", ", AllowedDocumentExtensions)}.");
        }

        // Replace any previously uploaded document for this contract.
        if (!string.IsNullOrEmpty(contract.DocumentPath))
        {
            _fileStorageService.DeleteFile(contract.DocumentPath);
        }

        var relativePath = await _fileStorageService.SaveFileAsync(file, $"contracts/{contractId}");
        contract.DocumentPath = relativePath;
        contract.UpdatedAt = DateTime.UtcNow;

        _contractRepository.Update(contract);
        await _contractRepository.SaveChangesAsync();

        return ServiceResult<ContractDto>.SuccessResult(_mapper.Map<ContractDto>(contract));
    }
}