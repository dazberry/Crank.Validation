using System;
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

        /// <summary>
        /// Creates a ValidationSource for the passed source variable. Accepts additional validation options.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="validationOptions"></param>
        /// <returns>ValidationSource<TSource></returns>
        public ValidationSource<TSource> For<TSource>(TSource source, ValidationOptions validationOptions = default) =>
            new ValidationSource<TSource>(this, source, validationOptions ?? _validationOptions);


        /// <summary>
        /// Creates a ValidationSource for the passed source variable. Accepts additional validation options.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="optionsAction"></param>
        /// <returns>ValidationSource</returns>
        public ValidationSource<TSource> For<TSource>(TSource source, Action<ValidationOptions> optionsAction = null)
        {
            var validationOptions = new ValidationOptions
            {
                StopApplyingRulesAfterFailure = _validationOptions.StopApplyingRulesAfterFailure
            };
            optionsAction?.Invoke(validationOptions);
            return new ValidationSource<TSource>(this, source, validationOptions);
        }

        /// <summary>
        /// Creates a ValidationSource for the passed source variable.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns>ValidationSource<TSource></returns>
        public ValidationSource<TSource> For<TSource>(TSource source) =>
            new ValidationSource<TSource>(this, source, _validationOptions);

        /// <summary>
        /// Try to get the rule specified by the TValidationRule
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <param name="validationRule"></param>
        /// <returns></returns>
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
