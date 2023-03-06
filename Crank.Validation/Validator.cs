using System.Collections.Generic;
using System.Linq;

namespace Crank.Validation
{
    public class Validation
    {
        private readonly IEnumerable<IValidationRule> _validationRules;
        private readonly ValidationOptions _validationOptions;

        public Validation(IEnumerable<IValidationRule> validationRules, ValidationOptions validationOptions = null)
        {
            _validationRules = validationRules;
            _validationOptions = validationOptions ?? new ValidationOptions();
        }

        public ValidationSource<TSource> For<TSource>(TSource source) =>
            new ValidationSource<TSource>(this, source, _validationOptions);

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
