using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : BaseApiController
{
    private readonly IContractService _contractService;
    private readonly IValidator<CreateContractDto> _createValidator;
    private readonly IValidator<UpdateContractDto> _updateValidator;

    public ContractsController(
        IContractService contractService,
        IValidator<CreateContractDto> createValidator,
        IValidator<UpdateContractDto> updateValidator)
    {
        _contractService = contractService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contracts = await _contractService.GetAllForFreelancerAsync(CurrentUserId);
        return Ok(contracts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _contractService.GetByIdAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _contractService.CreateAsync(CurrentUserId, dto);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContractDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _contractService.UpdateAsync(CurrentUserId, id, dto);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _contractService.DeleteAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/document")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file)
    {
        var result = await _contractService.UploadDocumentAsync(CurrentUserId, id, file);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    // POST {id}/analyze and GET {id}/analysis are added in the AI step,
    // rounding this controller out to 8 endpoints total.
}