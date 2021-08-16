using FluentValidation;

namespace EagleRock.Model.Validation
{
    public class CoordinatesValidator : AbstractValidator<Coordinate>
    {
        public CoordinatesValidator()
        {
            RuleFor(x => x.Latitude).InclusiveBetween(-90.0, 90.0);
            RuleFor(x => x.Longitude).InclusiveBetween(-180.0, 180.0);
        }
    }
}