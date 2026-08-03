using FreelanceFlow.Backend.DTOs.Milestones;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface IMilestoneService
{
    Task<ServiceResult<IReadOnlyList<MilestoneDto>>> GetAllForContractAsync(Guid freelancerId, Guid contractId);

    Task<ServiceResult<MilestoneDto>> CreateAsync(Guid freelancerId, Guid contractId, CreateMilestoneDto dto);

    Task<ServiceResult<MilestoneDto>> UpdateAsync(
        Guid freelancerId, Guid contractId, Guid milestoneId, UpdateMilestoneDto dto);

    /// <summary>
    /// Marks the milestone Completed and generates a draft Invoice for its
    /// amount, linked back via Invoice.MilestoneId. Fails if the milestone
    /// is already completed (idempotency — no duplicate invoices).
    /// </summary>
    Task<ServiceResult<MilestoneCompleteResultDto>> CompleteAsync(
        Guid freelancerId, Guid contractId, Guid milestoneId);
}