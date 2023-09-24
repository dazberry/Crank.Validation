using Crank.Validation.Tests.Models;
using System;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatComparesAgainstAGuidValue : IValidationRule<AnOtherSourceModel, Guid>
    {
        public IValidationResult ApplyTo(AnOtherSourceModel source, Guid inputValue)
        {
            return ValidationResult.Set(
                source?.AGuidValue == inputValue,
                "values do not match")
                    .WithValue(inputValue);
        }
    }
}
