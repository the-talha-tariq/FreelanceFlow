using FluentValidation;
using FreelanceFlow.Backend.DTOs.Milestones;

namespace FreelanceFlow.Backend.Validators.Milestones;

public class CreateMilestoneDtoValidator : AbstractValidator<CreateMilestoneDto>
{
    public CreateMilestoneDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DueDate).NotEmpty();
    }
}