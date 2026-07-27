using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Backend.DTOs.Clients;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : BaseApiController
{
    private readonly IClientService _clientService;
    private readonly IValidator<CreateClientDto> _createValidator;
    private readonly IValidator<UpdateClientDto> _updateValidator;

    public ClientsController(
        IClientService clientService,
        IValidator<CreateClientDto> createValidator,
        IValidator<UpdateClientDto> updateValidator)
    {
        _clientService = clientService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _clientService.GetAllForFreelancerAsync(CurrentUserId);
        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _clientService.GetByIdAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _clientService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _clientService.UpdateAsync(CurrentUserId, id, dto);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        var result = await _clientService.SoftDeleteAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return NoContent();
    }
}