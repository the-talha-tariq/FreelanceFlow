using AutoMapper;
using FreelanceFlow.Backend.DTOs.Milestones;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Models.Enums;
using FreelanceFlow.Backend.Repositories.Interfaces;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class MilestoneService : IMilestoneService
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public MilestoneService(
        IMilestoneRepository milestoneRepository,
        IContractRepository contractRepository,
        IInvoiceRepository invoiceRepository,
        IMapper mapper)
    {
        _milestoneRepository = milestoneRepository;
        _contractRepository = contractRepository;
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<MilestoneDto>>> GetAllForContractAsync(Guid freelancerId, Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<IReadOnlyList<MilestoneDto>>.FailureResult("Contract not found.");
        }

        var milestones = await _milestoneRepository.GetByContractIdAsync(contractId);
        return ServiceResult<IReadOnlyList<MilestoneDto>>.SuccessResult(_mapper.Map<IReadOnlyList<MilestoneDto>>(milestones));
    }

    public async Task<ServiceResult<MilestoneDto>> CreateAsync(Guid freelancerId, Guid contractId, CreateMilestoneDto dto)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null || contract.FreelancerId != freelancerId)
        {
            return ServiceResult<MilestoneDto>.FailureResult("Contract not found.");
        }

        var milestone = _mapper.Map<Milestone>(dto);
        milestone.Id = Guid.NewGuid();
        milestone.ContractId = contractId;
        milestone.Status = MilestoneStatus.Pending;
        milestone.CreatedAt = DateTime.UtcNow;
        milestone.UpdatedAt = DateTime.UtcNow;

        await _milestoneRepository.AddAsync(milestone);
        await _milestoneRepository.SaveChangesAsync();

        return ServiceResult<MilestoneDto>.SuccessResult(_mapper.Map<MilestoneDto>(milestone));
    }

    public async Task<ServiceResult<MilestoneDto>> UpdateAsync(
        Guid freelancerId, Guid contractId, Guid milestoneId, UpdateMilestoneDto dto)
    {
        var milestone = await _milestoneRepository.GetWithContractAsync(milestoneId);
        if (milestone == null || milestone.ContractId != contractId || milestone.Contract.FreelancerId != freelancerId)
        {
            return ServiceResult<MilestoneDto>.FailureResult("Milestone not found.");
        }

        if (milestone.Status == MilestoneStatus.Completed)
        {
            return ServiceResult<MilestoneDto>.FailureResult(
                "This milestone is already completed and its invoice has been generated — it can no longer be edited.");
        }

        _mapper.Map(dto, milestone);
        milestone.UpdatedAt = DateTime.UtcNow;

        _milestoneRepository.Update(milestone);
        await _milestoneRepository.SaveChangesAsync();

        return ServiceResult<MilestoneDto>.SuccessResult(_mapper.Map<MilestoneDto>(milestone));
    }

    public async Task<ServiceResult<MilestoneCompleteResultDto>> CompleteAsync(
        Guid freelancerId, Guid contractId, Guid milestoneId)
    {
        var milestone = await _milestoneRepository.GetWithContractAsync(milestoneId);
        if (milestone == null || milestone.ContractId != contractId || milestone.Contract.FreelancerId != freelancerId)
        {
            return ServiceResult<MilestoneCompleteResultDto>.FailureResult("Milestone not found.");
        }

        if (milestone.Status == MilestoneStatus.Completed)
        {
            return ServiceResult<MilestoneCompleteResultDto>.FailureResult(
                "This milestone has already been completed.");
        }

        milestone.Status = MilestoneStatus.Completed;
        milestone.CompletedAt = DateTime.UtcNow;
        milestone.UpdatedAt = DateTime.UtcNow;
        _milestoneRepository.Update(milestone);

        var invoiceCount = await _invoiceRepository.GetInvoiceCountForFreelancerAsync(freelancerId);
        var invoiceNumber = $"INV-{invoiceCount + 1:D4}";

        var issueDate = DateTime.UtcNow;
        var client = milestone.Contract.Client;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            FreelancerId = freelancerId,
            ClientId = milestone.Contract.ClientId,
            MilestoneId = milestone.Id,
            InvoiceNumber = invoiceNumber,
            IssueDate = issueDate,
            DueDate = issueDate.AddDays(client.PaymentTermsDays),
            SubTotal = milestone.Amount,
            TaxPercent = 0,
            TaxAmount = 0,
            TotalAmount = milestone.Amount,
            Status = InvoiceStatus.Draft,
            CreatedAt = issueDate,
            UpdatedAt = issueDate,
            LineItems = new List<InvoiceLineItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = $"Milestone: {milestone.Title}",
                    Quantity = 1,
                    UnitPrice = milestone.Amount,
                    Total = milestone.Amount
                }
            }
        };

        await _invoiceRepository.AddAsync(invoice);

        // One SaveChangesAsync commits both the milestone update and the new
        // invoice/line item — both repositories share the same scoped
        // AppDbContext instance for this request.
        await _invoiceRepository.SaveChangesAsync();

        // Wire up the navigation on the in-memory entity so mapping below
        // reflects the invoice we just created without a re-query.
        milestone.Invoice = invoice;

        return ServiceResult<MilestoneCompleteResultDto>.SuccessResult(new MilestoneCompleteResultDto
        {
            Milestone = _mapper.Map<MilestoneDto>(milestone),
            GeneratedInvoiceId = invoice.Id,
            GeneratedInvoiceNumber = invoice.InvoiceNumber
        });
    }
}