namespace Application;

using FluentValidation;
using System.Globalization;

public sealed class CreateOrderDeliveryCommandValidation : AbstractValidator<CreateOrderDeliveryCommand>
{
    public CreateOrderDeliveryCommandValidation()
    {
        RuleFor(x => x.Payload)
            .NotNull()
            .SetValidator(new TMSDeliveryEventRequestValidator());
    }
}

public sealed class TMSDeliveryEventRequestValidator : AbstractValidator<TMSDeliveryEventRequest>
{
    public TMSDeliveryEventRequestValidator()
    {
        RuleFor(x => x.ServiceType).NotEmpty();
        RuleFor(x => x.DispatchType).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.SubStatus).NotEmpty();
        RuleFor(x => x.VehicleCode).NotEmpty();
        RuleFor(x => x.CourierName).NotEmpty();

        RuleFor(x => x.Details)
            .NotNull()
            .SetValidator(new DeliveryDetailsRequestValidator());

        RuleFor(x => x.EventDate)
            .NotEmpty()
            .Must(value => DateTime.TryParseExact(
                value,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
            .WithMessage("eventDate debe tener el formato yyyy-MM-dd HH:mm:ss.");
    }
}

public sealed class DeliveryDetailsRequestValidator : AbstractValidator<DeliveryDetailsRequest?>
{
    public DeliveryDetailsRequestValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x!.OrderNumber).NotEmpty();
                RuleFor(x => x!.TrackingNumber).NotEmpty();
                RuleFor(x => x!.ClientCode).NotEmpty();
                RuleFor(x => x!.ClientName).NotEmpty();
                RuleFor(x => x!.ReceivedBy).NotEmpty();

                RuleForEach(x => x!.Evidences)
                    .SetValidator(new EvidenceRequestValidator());
            });
    }
}

public sealed class EvidenceRequestValidator : AbstractValidator<EvidenceRequest?>
{
    public EvidenceRequestValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x!.Label).NotEmpty();
                RuleFor(x => x!.FileType).NotEmpty();
                RuleFor(x => x!.FileName).NotEmpty();

                RuleFor(x => x!.Url)
                    .NotEmpty()
                    .Must(value =>
                        Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                        (uri.Scheme == Uri.UriSchemeHttps ||
                         uri.Scheme == Uri.UriSchemeHttp))
                    .WithMessage("url debe ser una dirección HTTP o HTTPS absoluta.");
            });
    }
}