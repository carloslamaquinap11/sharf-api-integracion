namespace Application;

using FluentValidation;
using System.Globalization;

public sealed class UpdateOrderDeliveryCommandValidation : AbstractValidator<UpdateOrderDeliveryCommand>
{
    public UpdateOrderDeliveryCommandValidation()
    {
        RuleFor(x => x.Payload)
            .NotNull()
            .SetValidator(new TMSDeliveryEventRequestValidator());
    }
}