using AutoMapper;
using FreelanceFlow.Backend.DTOs.Invoices;
using FreelanceFlow.Backend.ExternalServices;
using FreelanceFlow.Backend.Helpers;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Models.Enums;
using FreelanceFlow.Backend.Repositories.Interfaces;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IInvoicePdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IFileStorageService fileStorageService,
        IInvoicePdfService pdfService,
        IEmailService emailService,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _fileStorageService = fileStorageService;
        _pdfService = pdfService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InvoiceDto>> GetAllForFreelancerAsync(Guid freelancerId)
    {
        var invoices = await _invoiceRepository.GetByFreelancerIdAsync(freelancerId);
        return _mapper.Map<IReadOnlyList<InvoiceDto>>(invoices);
    }

    public async Task<ServiceResult<InvoiceDetailDto>> GetByIdAsync(Guid freelancerId, Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || invoice.FreelancerId != freelancerId)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult("Invoice not found.");
        }

        return ServiceResult<InvoiceDetailDto>.SuccessResult(_mapper.Map<InvoiceDetailDto>(invoice));
    }

    public async Task<ServiceResult<InvoiceDetailDto>> CreateAsync(Guid freelancerId, CreateInvoiceDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (client == null || client.FreelancerId != freelancerId)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult("Client not found.");
        }

        var invoiceCount = await _invoiceRepository.GetInvoiceCountForFreelancerAsync(freelancerId);
        var issueDate = DateTime.UtcNow;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            FreelancerId = freelancerId,
            ClientId = dto.ClientId,
            MilestoneId = null, // manual invoices are never milestone-linked
            InvoiceNumber = $"INV-{invoiceCount + 1:D4}",
            IssueDate = issueDate,
            DueDate = dto.DueDate,
            Status = InvoiceStatus.Draft,
            Notes = dto.Notes,
            CreatedAt = issueDate,
            UpdatedAt = issueDate,
            LineItems = new List<InvoiceLineItem>()
        };

        RecalculateLineItemsAndTotals(invoice, dto.LineItems, dto.TaxPercent);

        await _invoiceRepository.AddAsync(invoice);
        await _invoiceRepository.SaveChangesAsync();

        invoice.Client = client;
        return ServiceResult<InvoiceDetailDto>.SuccessResult(_mapper.Map<InvoiceDetailDto>(invoice));
    }

    public async Task<ServiceResult<InvoiceDetailDto>> UpdateAsync(Guid freelancerId, Guid invoiceId, UpdateInvoiceDto dto)
    {
        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || invoice.FreelancerId != freelancerId)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult("Invoice not found.");
        }

        if (invoice.Status != InvoiceStatus.Draft)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult(
                $"Only draft invoices can be edited — this invoice is {invoice.Status}.");
        }

        invoice.DueDate = dto.DueDate;
        invoice.Notes = dto.Notes;
        invoice.UpdatedAt = DateTime.UtcNow;

        RecalculateLineItemsAndTotals(invoice, dto.LineItems, dto.TaxPercent);

        _invoiceRepository.Update(invoice);
        await _invoiceRepository.SaveChangesAsync();

        return ServiceResult<InvoiceDetailDto>.SuccessResult(_mapper.Map<InvoiceDetailDto>(invoice));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid freelancerId, Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
        if (invoice == null || invoice.FreelancerId != freelancerId)
        {
            return ServiceResult<bool>.FailureResult("Invoice not found.");
        }

        if (invoice.Status != InvoiceStatus.Draft)
        {
            return ServiceResult<bool>.FailureResult(
                $"Only draft invoices can be deleted — this invoice is {invoice.Status}.");
        }

        if (!string.IsNullOrEmpty(invoice.PdfPath))
        {
            _fileStorageService.DeleteFile(invoice.PdfPath);
        }

        // LineItems and Payments cascade-delete via the FK configuration in
        // InvoiceConfiguration.
        _invoiceRepository.Remove(invoice);
        await _invoiceRepository.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<InvoicePdfResult>> GetPdfAsync(Guid freelancerId, Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || invoice.FreelancerId != freelancerId)
        {
            return ServiceResult<InvoicePdfResult>.FailureResult("Invoice not found.");
        }

        var pdfBytes = _pdfService.GeneratePdf(invoice);

        // Cache the first-generated copy to disk so PdfPath is populated;
        // the bytes returned here are always freshly rendered, though, so a
        // Draft invoice edited after an earlier download still reflects the
        // latest line items.
        if (string.IsNullOrEmpty(invoice.PdfPath))
        {
            invoice.PdfPath = await SavePdfToDiskAsync(invoice.Id, pdfBytes);
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoiceRepository.Update(invoice);
            await _invoiceRepository.SaveChangesAsync();
        }

        return ServiceResult<InvoicePdfResult>.SuccessResult(new InvoicePdfResult(pdfBytes, $"{invoice.InvoiceNumber}.pdf"));
    }

    public async Task<ServiceResult<InvoiceDetailDto>> SendAsync(Guid freelancerId, Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || invoice.FreelancerId != freelancerId)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult("Invoice not found.");
        }

        if (invoice.Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult(
                $"Cannot send an invoice that is already {invoice.Status}.");
        }

        var pdfBytes = _pdfService.GeneratePdf(invoice);

        // Always refresh the cached PDF on send, so what's emailed matches
        // what a client would download right now.
        invoice.PdfPath = await SavePdfToDiskAsync(invoice.Id, pdfBytes);

        try
        {
            await _emailService.SendInvoiceEmailAsync(
                invoice.Client.Email,
                invoice.Client.Name,
                invoice.InvoiceNumber,
                invoice.TotalAmount,
                invoice.Freelancer.Currency.ToString(),
                pdfBytes);
        }
        catch (Exception ex)
        {
            return ServiceResult<InvoiceDetailDto>.FailureResult($"Failed to send invoice email: {ex.Message}");
        }

        if (invoice.Status == InvoiceStatus.Draft)
        {
            invoice.Status = InvoiceStatus.Sent;
        }
        invoice.UpdatedAt = DateTime.UtcNow;

        _invoiceRepository.Update(invoice);
        await _invoiceRepository.SaveChangesAsync();

        return ServiceResult<InvoiceDetailDto>.SuccessResult(_mapper.Map<InvoiceDetailDto>(invoice));
    }

    private async Task<string> SavePdfToDiskAsync(Guid invoiceId, byte[] bytes) =>
        await _fileStorageService.SaveBytesAsync(bytes, "invoices", $"{invoiceId}.pdf");

    /// <summary>
    /// Replaces the invoice's line items with the given input and
    /// recomputes SubTotal/TaxAmount/TotalAmount server-side. Per-line
    /// Total is never trusted from the client — always Quantity * UnitPrice.
    /// </summary>
    private static void RecalculateLineItemsAndTotals(
        Invoice invoice, List<InvoiceLineItemInputDto> lineItemInputs, decimal taxPercent)
    {
        invoice.LineItems.Clear();

        var subTotal = 0m;
        foreach (var input in lineItemInputs)
        {
            var total = Math.Round(input.Quantity * input.UnitPrice, 2);
            subTotal += total;

            invoice.LineItems.Add(new InvoiceLineItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                Description = input.Description,
                Quantity = input.Quantity,
                UnitPrice = input.UnitPrice,
                Total = total
            });
        }

        var taxAmount = Math.Round(subTotal * taxPercent / 100m, 2);

        invoice.SubTotal = subTotal;
        invoice.TaxPercent = taxPercent;
        invoice.TaxAmount = taxAmount;
        invoice.TotalAmount = subTotal + taxAmount;
    }
}