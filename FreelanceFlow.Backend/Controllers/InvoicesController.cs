using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Backend.DTOs.Invoices;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : BaseApiController
{
    private readonly IInvoiceService _invoiceService;
    private readonly IValidator<CreateInvoiceDto> _createValidator;
    private readonly IValidator<UpdateInvoiceDto> _updateValidator;

    public InvoicesController(
        IInvoiceService invoiceService,
        IValidator<CreateInvoiceDto> createValidator,
        IValidator<UpdateInvoiceDto> updateValidator)
    {
        _invoiceService = invoiceService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _invoiceService.GetAllForFreelancerAsync(CurrentUserId);
        return Ok(invoices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _invoiceService.GetByIdAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _invoiceService.CreateAsync(CurrentUserId, dto);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _invoiceService.UpdateAsync(CurrentUserId, id, dto);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _invoiceService.DeleteAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetPdf(Guid id)
    {
        var result = await _invoiceService.GetPdfAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return NotFound(result.Errors);
        }

        return File(result.Data!.Bytes, "application/pdf", result.Data.FileName);
    }

    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> Send(Guid id)
    {
        var result = await _invoiceService.SendAsync(CurrentUserId, id);
        if (!result.Success)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}