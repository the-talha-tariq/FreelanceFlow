using FluentValidation;
using FreelanceFlow.Backend.DTOs.Invoices;

namespace FreelanceFlow.Backend.Validators.Invoices;

public class InvoiceLineItemInputDtoValidator : AbstractValidator<InvoiceLineItemInputDto>
{
    public InvoiceLineItemInputDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}