using FluentValidation;
using FreelanceFlow.Backend.DTOs.Invoices;

namespace FreelanceFlow.Backend.Validators.Invoices;

public class UpdateInvoiceDtoValidator : AbstractValidator<UpdateInvoiceDto>
{
    public UpdateInvoiceDtoValidator()
    {
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.TaxPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.Notes).MaximumLength(2000);

        RuleFor(x => x.LineItems).NotEmpty().WithMessage("An invoice needs at least one line item.");
        RuleForEach(x => x.LineItems).SetValidator(new InvoiceLineItemInputDtoValidator());
    }
}