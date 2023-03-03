using System.Collections.Generic;
using System.Linq;

namespace Crank.Validation
{
    public class Validation
    {
        private IEnumerable<IValidationRule> _validationRules;
        public Validation(IEnumerable<IValidationRule> validationRules)
        {
            _validationRules = validationRules;
        }

        public ValidationSource<TSource> For<TSource>(TSource source) =>
            new ValidationSource<TSource>(this, source);

        public bool TryGetRule<TValidationRule>(out TValidationRule validationRule)
            where TValidationRule : IValidationRule

        {
            var ruleType = typeof(TValidationRule);

            var rule = _validationRules.FirstOrDefault(x => x is TValidationRule);
            if (rule != null)
            {
                validationRule = (TValidationRule)rule;
                return true;
            }

            validationRule = default!;
            return false;
        }
    }


}
