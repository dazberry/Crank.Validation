using System;
using System.Collections.Generic;
using System.Linq;

namespace Crank.Validation
{
    public class ValidationSource<TSource>
    {
        public TSource Source { get; private set; }
        private readonly Validation _validation;

        private readonly IDictionary<Type, IValidationResult> _results = new Dictionary<Type, IValidationResult>();

        public IEnumerable<IValidationResult> Failures => _results.Values.Where(x => !x.Passed);
        public IEnumerable<IValidationResult> Results => _results.Values;
        public IValidationResult Result<TValidationRule>() =>
            _results.TryGetValue(typeof(TValidationRule), out IValidationResult validationResult)
                ? validationResult : default!;

        public bool Passing => _results.Values.All(x => x.Passed);
        public bool Passed<TValidationRule>() =>
            Result<TValidationRule>()?.Passed ?? false;

        public void Reset() => _results.Clear();


        public bool TryGetValue<TValidationRule, TValueType>(out TValueType value)
        {
            var result = Result<TValidationRule>();
            if (result != null)
                return result.TryGetValue(out value);
            value = default!;
            return false;
        }

        public bool TryGetValue<TValueType>(out TValueType value)
        {
            TValueType valueResult = default!;
            var result = _results.Values.FirstOrDefault(res => res.TryGetValue(out valueResult));
            value = valueResult;
            return result != null;
        }


        public ValidationSource(Validation validation, TSource source)
        {
            Source = source;
            _validation = validation;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule>()
            where TValidationRule : IValidationRule<TSource>

        {
            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                _results.Add(typeof(TValidationRule), new ValidationResult(false, "Rule not found"));
                return this;
            }

            var result = validationRule.ApplyTo(Source);
            _results.Add(typeof(TValidationRule), result);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule>(out IValidationResult validationResult)
            where TValidationRule : IValidationRule<TSource>

        {
            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                validationResult = default;
                _results.Add(typeof(TValidationRule), new ValidationResult(false, "Rule not found"));
                return this;
            }

            validationResult = validationRule.ApplyTo(Source);
            _results.Add(typeof(TValidationRule), validationResult);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TComparison>(TComparison comparisonValue)
            where TValidationRule : IValidationRule<TSource, TComparison>
        {
            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                _results.Add(typeof(TValidationRule), new ValidationResult(false, "Rule not found"));
                return this;
            }

            var result = validationRule.ApplyTo(Source, comparisonValue);
            _results.Add(typeof(TValidationRule), result);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TComparison>(TComparison comparisonValue, out IValidationResult validationResult)
            where TValidationRule : IValidationRule<TSource, TComparison>
        {
            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                validationResult = default!;
                _results.Add(typeof(TValidationRule), new ValidationResult(false, "Rule not found"));
                return this;
            }

            validationResult = validationRule.ApplyTo(Source, comparisonValue);
            _results.Add(typeof(TValidationRule), validationResult);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TOutValue>(out TOutValue outValue)
            where TValidationRule : IValidationRuleWithOutValue<TSource, TOutValue>
        {

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                outValue = default!;
                _results.Add(typeof(TValidationRule), new ValidationResult(false, "Rule not found"));
                return this;
            }

            var result = validationRule.ApplyTo(Source, out outValue);
            _results.Add(typeof(TValidationRule), result);
            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TOutValue, TAdditionalOutValue>(out TOutValue outValue, out TAdditionalOutValue additionalOutValue)
                    where TValidationRule : IValidationRuleWithOutValues<TSource, TOutValue, TAdditionalOutValue>
        {
            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                outValue = default!;
                additionalOutValue = default!;
                _results.Add(typeof(TValidationRule), new ValidationResult(false, "Rule not found"));
                return this;
            }

            var result = validationRule.ApplyTo(Source, out outValue, out additionalOutValue);
            _results.Add(typeof(TValidationRule), result);
            return this;
        }
    }
}
