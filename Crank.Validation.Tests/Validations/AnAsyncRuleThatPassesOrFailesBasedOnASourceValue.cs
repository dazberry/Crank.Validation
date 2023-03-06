using Crank.Validation.Tests.Models;
using System.Threading.Tasks;

namespace Crank.Validation.Tests.Validations
{
    public class AnAsyncRuleThatPassesOrFailesBasedOnASourceValue : IValidationRuleAsync<SourceModel>
    {
        public async Task<IValidationResult> ApplyTo(SourceModel source)
        {
            await Task.CompletedTask;

            return ValidationResult.Set(bool.Parse(source.AStringValue), "Validation Failed");
        }
    }
}
