namespace FreelanceFlow.Backend.DTOs.Invoices;

public class UpdateInvoiceDto
{
    public DateTime DueDate { get; set; }
    public decimal TaxPercent { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceLineItemInputDto> LineItems { get; set; } = new();
}