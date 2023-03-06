using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crank.Validation
{

    public class ValidationSource<TSource>
    {
        private readonly ValidationOptions _validationOptions;

        public const string RuleNotFound = "Rule not found";
        public const string RuleNotApplied = "Rule not applied - due to previous failing validations";

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

        public ValidationSource(Validation validation, TSource source, ValidationOptions validationOptions = null)
        {
            Source = source;
            _validation = validation;
            _validationOptions = validationOptions ?? new ValidationOptions();
        }

        private bool StopApplyRulesAfterFailureIfFlagSet() =>
             _validationOptions.StopApplyingRulesAfterFailed && this.Failures.Any();


        private void AddAFailedValidationResult<TValidationRule>(string errorMessage) =>
            _results.Add(typeof(TValidationRule), new ValidationResult(false, errorMessage));

        private void AddFailedValidationResult<TValidationRule>(string errorMessage, out IValidationResult validationResult)
        {
            validationResult = new ValidationResult(false, errorMessage);
            _results.Add(typeof(TValidationRule), new ValidationResult(false, errorMessage));
        }

        private ValidationSource<TSource> ValidationResultRuleNotFound<TValidationRule>()
        {
            AddAFailedValidationResult<TValidationRule>(RuleNotFound);
            return this;
        }

        private ValidationSource<TSource> ValidationResultRuleNotApplied<TValidationRule>()
        {
            AddAFailedValidationResult<TValidationRule>(RuleNotApplied);
            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule>()
            where TValidationRule : IValidationRule<TSource>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();


            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var result = validationRule.ApplyTo(Source);
            _results.Add(typeof(TValidationRule), result);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule>(out IValidationResult validationResult)
            where TValidationRule : IValidationRule<TSource>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
            {
                AddFailedValidationResult<TValidationRule>(RuleNotApplied, out validationResult);
                return this;
            }

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                AddFailedValidationResult<TValidationRule>(RuleNotFound, out validationResult);
                return this;
            }

            validationResult = validationRule.ApplyTo(Source);
            _results.Add(typeof(TValidationRule), validationResult);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TInputValue>(TInputValue inputValue)
            where TValidationRule : IValidationRule<TSource, TInputValue>
        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var result = validationRule.ApplyTo(Source, inputValue);
            _results.Add(typeof(TValidationRule), result);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TInputValue>(TInputValue inputValue, out IValidationResult validationResult)
            where TValidationRule : IValidationRule<TSource, TInputValue>
        {
            validationResult = default; //[dd]

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            validationResult = validationRule.ApplyTo(Source, inputValue);
            _results.Add(typeof(TValidationRule), validationResult);

            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TOutValue>(out TOutValue outValue)
            where TValidationRule : IValidationRuleWithOutValue<TSource, TOutValue>
        {
            outValue = default!;

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var result = validationRule.ApplyTo(Source, out outValue);
            _results.Add(typeof(TValidationRule), result);
            return this;
        }

        public ValidationSource<TSource> ApplyRule<TValidationRule, TOutValue, TAdditionalOutValue>(out TOutValue outValue, out TAdditionalOutValue additionalOutValue)
                    where TValidationRule : IValidationRuleWithOutValues<TSource, TOutValue, TAdditionalOutValue>
        {
            outValue = default!;
            additionalOutValue = default!;

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var result = validationRule.ApplyTo(Source, out outValue, out additionalOutValue);
            _results.Add(typeof(TValidationRule), result);
            return this;
        }

        public async Task<ValidationSource<TSource>> ApplyRuleAsync<TValidationRuleAsync>()
           where TValidationRuleAsync : IValidationRuleAsync<TSource>

        {

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRuleAsync>();

            if (!_validation.TryGetRule<TValidationRuleAsync>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRuleAsync>();

            var result = await validationRule.ApplyTo(Source);
            _results.Add(typeof(TValidationRuleAsync), result);

            return this;
        }

        public async Task<ValidationSource<TSource>> ApplyRuleAsync<TValidationRuleAsync, TInputValue>(TInputValue inputValue)
            where TValidationRuleAsync : IValidationRuleAsync<TSource, TInputValue>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRuleAsync>();

            if (!_validation.TryGetRule<TValidationRuleAsync>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRuleAsync>();

            var result = await validationRule.ApplyTo(Source, inputValue);
            _results.Add(typeof(TValidationRuleAsync), result);

            return this;
        }

        public async Task<ValidationSource<TSource>> ApplyRuleAsync<TValidationRuleAsync, TInputValue, TAdditionalInputValue>(TInputValue inputValue, TAdditionalInputValue additionalValue)
            where TValidationRuleAsync : IValidationRuleAsync<TSource, TInputValue, TAdditionalInputValue>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRuleAsync>();

            if (!_validation.TryGetRule<TValidationRuleAsync>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRuleAsync>();

            var result = await validationRule.ApplyTo(Source, inputValue, additionalValue);
            _results.Add(typeof(TValidationRuleAsync), result);

            return this;
        }
    }
}
