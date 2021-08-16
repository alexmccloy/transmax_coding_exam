using FluentValidation;

namespace EagleRock.Model.Validation
{
    public class EagleBotPayloadValidator : AbstractValidator<EagleBotPayload>
    {
        public EagleBotPayloadValidator()
        {
            RuleFor(x => x.EagleBotId).NotEmpty();
            RuleFor(x => x.Coordinates).SetValidator(new CoordinatesValidator());
            RuleFor(x => x.StreetName).NotEmpty();
            RuleFor(x => x.TrafficFlowRate).GreaterThan(0);
            RuleFor(x => x.AverageVehicleSpeed).InclusiveBetween(0, 250);
        }
    }
}