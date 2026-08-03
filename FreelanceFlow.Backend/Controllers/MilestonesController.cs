using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Backend.DTOs.Milestones;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Controllers;

[ApiController]
[Route("api/contracts/{contractId:guid}/milestones")]
public class MilestonesController : BaseApiController
{
    private readonly IMilestoneService _milestoneService;
    private readonly IValidator<CreateMilestoneDto> _createValidator;
    private readonly IValidator<UpdateMilestoneDto> _updateValidator;

    public MilestonesController(
        IMilestoneService milestoneService,
        IValidator<CreateMilestoneDto> createValidator,
        IValidator<UpdateMilestoneDto> updateValidator)
    {
        _milestoneService = milestoneService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid contractId)
    {
        var result = await _milestoneService.GetAllForContractAsync(CurrentUserId, contractId);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid contractId, [FromBody] CreateMilestoneDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _milestoneService.CreateAsync(CurrentUserId, contractId, dto);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return CreatedAtAction(nameof(GetAll), new { contractId }, result.Data);
    }

    [HttpPut("{milestoneId:guid}")]
    public async Task<IActionResult> Update(Guid contractId, Guid milestoneId, [FromBody] UpdateMilestoneDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _milestoneService.UpdateAsync(CurrentUserId, contractId, milestoneId, dto);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost("{milestoneId:guid}/complete")]
    public async Task<IActionResult> Complete(Guid contractId, Guid milestoneId)
    {
        var result = await _milestoneService.CompleteAsync(CurrentUserId, contractId, milestoneId);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}