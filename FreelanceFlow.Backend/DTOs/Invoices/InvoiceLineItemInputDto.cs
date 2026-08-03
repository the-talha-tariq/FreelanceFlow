namespace FreelanceFlow.Backend.DTOs.Invoices;

public class InvoiceLineItemInputDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}