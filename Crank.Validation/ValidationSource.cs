using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crank.Validation
{

    public abstract class ValidationSource
    {
        private readonly ValidationOptions _validationOptions;

        public const string RuleNotFound = "Rule not found";
        public const string RuleNotApplied = "Rule not applied - due to previous failing validations";

        //public TSource Source { get; private set; }

        protected readonly Validation _validation;

        protected readonly IDictionary<Type, IValidationResult> _results;

        /// <summary>
        /// An enumeration of failed results
        /// </summary>
        public IEnumerable<IValidationResult> Failures => _results.Values.Where(x => !x.Passed);

        /// <summary>
        /// An enumeration of all results
        /// </summary>
        public IEnumerable<IValidationResult> Results => _results.Values;

        /// <summary>
        /// Returns a validation result based on the supplied validation rule
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <returns></returns>
        public IValidationResult Result<TValidationRule>() =>
            _results.TryGetValue(typeof(TValidationRule), out IValidationResult validationResult)
                ? validationResult : default!;

        /// <summary>
        /// Check all rules have passed
        /// </summary>
        public bool Passing => _results.Values.All(x => x.Passed);
        public bool Passed<TValidationRule>() =>
            Result<TValidationRule>()?.Passed ?? false;

        /// <summary>
        /// Clear all validation results and reset state
        /// </summary>
        public void Reset() => _results.Clear();

        /// <summary>
        /// Try Get
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <typeparam name="TValueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<TValidationRule, TValueType>(out TValueType value)
        {
            var result = Result<TValidationRule>();
            if (result != null)
                return result.TryGetValue(out value);
            value = default!;
            return false;
        }

        /// <summary>
        /// Try Get
        /// </summary>
        /// <typeparam name="TValueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<TValueType>(out TValueType value)
        {
            TValueType valueResult = default!;
            var result = _results.Values.FirstOrDefault(res => res.TryGetValue(out valueResult));
            value = valueResult;
            return result != null;
        }

        public ValidationSource(Validation validation, ValidationOptions validationOptions = null)
        {
            _validation = validation;
            _validationOptions = validationOptions ?? new ValidationOptions();
            _results = new Dictionary<Type, IValidationResult>();
        }

        public ValidationSource(Validation validation, ValidationOptions validationOptions, IDictionary<Type, IValidationResult> results)
        {
            _validation = validation;
            _validationOptions = validationOptions ?? new ValidationOptions();
            _results = results ?? new Dictionary<Type, IValidationResult>();
        }

        protected bool StopApplyRulesAfterFailureIfFlagSet() =>
             _validationOptions.StopApplyingRulesAfterFailure && this.Failures.Any();

        protected void AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(IValidationResult validationResult)
        {
            if (validationResult is ValidationResultWithSourceAndRule resultWithSourceAndRule)
            {
                resultWithSourceAndRule.SetSourceAndRuleDetails<TSource, TValidationRule>();
            }

            _results.Add(typeof(TValidationRule), validationResult);
        }

        protected void AddAFailedValidationResult<TSource, TValidationRule>(string errorMessage) =>
            AddFailedValidationResult<TSource, TValidationRule>(errorMessage, out var _);

        protected void AddFailedValidationResult<TSource, TValidationRule>(string errorMessage, out IValidationResult validationResult)
        {
            validationResult = new ValidationResultWithSourceAndRule(false, errorMessage)
                .SetSourceAndRuleDetails<TSource, TValidationRule>();
            _results.Add(typeof(TValidationRule), validationResult);
        }


        /// <summary>
        /// Start applying rules against a new Validation Source
        /// </summary>
        /// <typeparam name="TNewSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>

        public ValidationSource<TNewSource> For<TNewSource>(TNewSource source) =>
            new ValidationSource<TNewSource>(_validation, source, _validationOptions, _results);
    }

    public class ValidationSource<TSource> : ValidationSource
    {
        /// <summary>
        /// Validation Source
        /// </summary>
        public TSource Source { get; private set; }

        public ValidationSource(Validation validation, TSource source, ValidationOptions validationOptions = null)
            : base(validation, validationOptions)
        {
            Source = source;
        }

        public ValidationSource(Validation validation, TSource source, ValidationOptions validationOptions, IDictionary<Type, IValidationResult> results)
            : base(validation, validationOptions, results)
        {
            Source = source;
        }

        private ValidationSource<TSource> ValidationResultRuleNotFound<TValidationRule>()
        {
            AddAFailedValidationResult<TSource, TValidationRule>(RuleNotFound);
            return this;
        }

        private ValidationSource<TSource> ValidationResultRuleNotApplied<TValidationRule>()
        {
            AddAFailedValidationResult<TSource, TValidationRule>(RuleNotApplied);
            return this;
        }


        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <returns></returns>
        public ValidationSource<TSource> ApplyRule<TValidationRule>()
            where TValidationRule : IValidationRule<TSource>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var validationResult = validationRule.ApplyTo(Source);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(validationResult);

            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        public ValidationSource<TSource> ApplyRule<TValidationRule>(out IValidationResult validationResult)
            where TValidationRule : IValidationRule<TSource>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
            {
                AddFailedValidationResult<TSource, TValidationRule>(RuleNotApplied, out validationResult);
                return this;
            }

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
            {
                AddFailedValidationResult<TSource, TValidationRule>(RuleNotFound, out validationResult);
                return this;
            }

            validationResult = validationRule.ApplyTo(Source);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(validationResult);

            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <typeparam name="TInputValue"></typeparam>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public ValidationSource<TSource> ApplyRule<TValidationRule, TInputValue>(TInputValue inputValue)
            where TValidationRule : IValidationRule<TSource, TInputValue>
        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var validationResult = validationRule.ApplyTo(Source, inputValue);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(validationResult);

            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <typeparam name="TInputValue"></typeparam>
        /// <param name="inputValue"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        public ValidationSource<TSource> ApplyRule<TValidationRule, TInputValue>(TInputValue inputValue, out IValidationResult validationResult)
            where TValidationRule : IValidationRule<TSource, TInputValue>
        {
            validationResult = default; //[dd]

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            validationResult = validationRule.ApplyTo(Source, inputValue);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(validationResult);

            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <typeparam name="TOutValue"></typeparam>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public ValidationSource<TSource> ApplyRule<TValidationRule, TOutValue>(out TOutValue outValue)
            where TValidationRule : IValidationRuleWithOutValue<TSource, TOutValue>
        {
            outValue = default!;

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var result = validationRule.ApplyTo(Source, out outValue);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(result);
            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRule"></typeparam>
        /// <typeparam name="TOutValue"></typeparam>
        /// <typeparam name="TAdditionalOutValue"></typeparam>
        /// <param name="outValue"></param>
        /// <param name="additionalOutValue"></param>
        /// <returns></returns>
        public ValidationSource<TSource> ApplyRule<TValidationRule, TOutValue, TAdditionalOutValue>(out TOutValue outValue, out TAdditionalOutValue additionalOutValue)
                    where TValidationRule : IValidationRuleWithOutValues<TSource, TOutValue, TAdditionalOutValue>
        {
            outValue = default!;
            additionalOutValue = default!;

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRule>();

            if (!_validation.TryGetRule<TValidationRule>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRule>();

            var validationResult = validationRule.ApplyTo(Source, out outValue, out additionalOutValue);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRule>(validationResult);
            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRuleAsync"></typeparam>
        /// <returns></returns>
        public async Task<ValidationSource<TSource>> ApplyRuleAsync<TValidationRuleAsync>()
           where TValidationRuleAsync : IValidationRuleAsync<TSource>

        {

            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRuleAsync>();

            if (!_validation.TryGetRule<TValidationRuleAsync>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRuleAsync>();

            var validationResult = await validationRule.ApplyTo(Source);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRuleAsync>(validationResult);

            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRuleAsync"></typeparam>
        /// <typeparam name="TInputValue"></typeparam>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public async Task<ValidationSource<TSource>> ApplyRuleAsync<TValidationRuleAsync, TInputValue>(TInputValue inputValue)
            where TValidationRuleAsync : IValidationRuleAsync<TSource, TInputValue>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRuleAsync>();

            if (!_validation.TryGetRule<TValidationRuleAsync>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRuleAsync>();

            var validationResult = await validationRule.ApplyTo(Source, inputValue);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRuleAsync>(validationResult);

            return this;
        }

        /// <summary>
        /// Apply the specified rule to the source type - if a matching rule is registered.
        /// </summary>
        /// <typeparam name="TValidationRuleAsync"></typeparam>
        /// <typeparam name="TInputValue"></typeparam>
        /// <typeparam name="TAdditionalInputValue"></typeparam>
        /// <param name="inputValue"></param>
        /// <param name="additionalValue"></param>
        /// <returns></returns>
        public async Task<ValidationSource<TSource>> ApplyRuleAsync<TValidationRuleAsync, TInputValue, TAdditionalInputValue>(TInputValue inputValue, TAdditionalInputValue additionalValue)
            where TValidationRuleAsync : IValidationRuleAsync<TSource, TInputValue, TAdditionalInputValue>

        {
            if (StopApplyRulesAfterFailureIfFlagSet())
                return ValidationResultRuleNotApplied<TValidationRuleAsync>();

            if (!_validation.TryGetRule<TValidationRuleAsync>(out var validationRule))
                return ValidationResultRuleNotFound<TValidationRuleAsync>();

            var validationResult = await validationRule.ApplyTo(Source, inputValue, additionalValue);
            AddAValidationResultAndSetSourceAndRule<TSource, TValidationRuleAsync>(validationResult);

            return this;
        }
    }
}
