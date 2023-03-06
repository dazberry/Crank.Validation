using Crank.Validation.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
