using Crank.Validation.Tests.Models;
using System.Threading.Tasks;

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

    public class ARuleThatPassesAndReturnsAStringValueAsync : IValidationRuleAsync<SourceModel>
    {
        public async Task<IValidationResult> ApplyTo(SourceModel source)
        {
            await Task.CompletedTask;

            return ValidationResult.Pass()
                .WithValue<string>(source?.AStringValue ?? "unspecified");
        }
    }
}
