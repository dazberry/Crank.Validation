using Crank.Validation.Tests.Models;
using System.Threading.Tasks;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatChecksTheSourceStringValue : IValidationRule<SourceModel, string>
    {
        public IValidationResult ApplyTo(SourceModel source, string inputValue)
        {
            return ValidationResult.Set(
                string.Equals(source?.AStringValue, inputValue),
                "values do not match");
        }
    }
}
