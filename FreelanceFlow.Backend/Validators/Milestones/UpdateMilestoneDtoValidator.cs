using FluentValidation;
using FreelanceFlow.Backend.DTOs.Milestones;
using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.Validators.Milestones;

public class UpdateMilestoneDtoValidator : AbstractValidator<UpdateMilestoneDto>
{
    public UpdateMilestoneDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DueDate).NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum()
            .NotEqual(MilestoneStatus.Completed)
            .WithMessage("Use POST /complete to mark a milestone complete — it also generates the invoice.");
    }
}