using Crank.Validation.Tests.Models;
using System;

namespace Crank.Validation.Tests.Validations
{
    public class ThisRuleIsNeverRegistered : IValidationRule<SourceModel>
    {
        public IValidationResult ApplyTo(SourceModel source)
        {
            throw new NotImplementedException();
        }
    }
}
