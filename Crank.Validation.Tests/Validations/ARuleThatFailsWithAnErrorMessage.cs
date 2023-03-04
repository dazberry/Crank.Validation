using Crank.Validation.Tests.Models;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatFailsWithAnErrorMessage : IValidationRule<SourceModel>
    {
        public IValidationResult ApplyTo(SourceModel source)
        {
            return ValidationResult.Fail(source?.AStringValue ?? "Unspecified Message");
        }
    }
}
