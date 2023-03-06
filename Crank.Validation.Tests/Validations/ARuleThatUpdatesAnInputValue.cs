using Crank.Validation.Tests.Models;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatUpdatesAnInputValue : IValidationRule<SourceModel, SourceModel>
    {
        public IValidationResult ApplyTo(SourceModel source, SourceModel inputValue)
        {
            inputValue.AStringValue = source.AStringValue;
            inputValue.AnIntegerValue = source.AnIntegerValue;
            return ValidationResult.Pass()
                .WithValue(inputValue);
        }
    }
}
