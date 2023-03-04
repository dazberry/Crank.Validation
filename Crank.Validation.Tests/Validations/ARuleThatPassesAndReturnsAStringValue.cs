using Crank.Validation.Tests.Models;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatPassesAndReturnsAStringValue : IValidationRule<SourceModel>
    {
        public IValidationResult ApplyTo(SourceModel source)
        {
            return ValidationResult.Pass()
                .WithValue<string>(source?.AStringValue ?? "unspecified");
        }
    }
}
