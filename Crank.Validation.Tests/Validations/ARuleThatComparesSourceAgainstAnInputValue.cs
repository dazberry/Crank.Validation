using Crank.Validation.Tests.Models;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatComparesSourceAgainstAnInputValue : IValidationRule<SourceModel, string>
    {
        public IValidationResult ApplyTo(SourceModel source, string inputValue)
        {
            return ValidationResult.Set(
                string.Equals(source?.AStringValue, inputValue),
                "values do not match")
                    .WithValue(inputValue);
        }
    }
}
