using Crank.Validation.Tests.Models;
using System.Threading.Tasks;

namespace Crank.Validation.Tests.Validations
{
    public class ARuleThatFailsWithAnErrorMessage : IValidationRule<SourceModel>
    {
        public IValidationResult ApplyTo(SourceModel source)
        {
            return ValidationResult.Fail(source?.AStringValue ?? "Unspecified Message");
        }
    }

    public class ARuleThatFailsWithAnErrorMessageAsync : IValidationRuleAsync<SourceModel>
    {
        public async Task<IValidationResult> ApplyTo(SourceModel source)
        {
            await Task.CompletedTask;

            return ValidationResult.Fail(source?.AStringValue ?? "Unspecified Message");
        }
    }
}
