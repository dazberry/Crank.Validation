using System;
using System.Collections.Generic;

namespace Crank.Validation
{
    public class ValidationResult : IValidationResult
    {
        public string Source { get; protected set; }
        public string Rule { get; protected set; }

        public bool Passed { get; }
        public string ErrorMessage { get; }

        private interface IValidationValue
        {
            Type StoredType { get; }

            public bool TryGetValue<T>(out T value)
            {
                value = default!;
                return false;
            }
        }
        private class ValidationValue<T> : IValidationValue
        {
            private T _value;

            public Type StoredType => typeof(T);

            public ValidationValue(T value)
            {
                _value = value;
            }

            public bool TryGetValue<TValue>(out TValue value)
            {
                if (typeof(T) == typeof(TValue))
                {
                    value = (TValue)Convert.ChangeType(_value, typeof(TValue));
                    return true;
                }

                value = default!;
                return false;
            }
        }

        private IDictionary<Type, IValidationValue> _values = null!;

        protected ValidationResult(bool passed, string errorMessage = "")
        {
            Passed = passed;
            ErrorMessage = errorMessage;
        }

        public bool TryGetValue<TValueType>(out TValueType value)
        {
            value = default!;
            if (_values == null)
                return false;

            var result =
                _values.TryGetValue(typeof(TValueType), out var genericValue) &&
                genericValue.TryGetValue(out value);

            return result;
        }

        public IValidationResult WithValue<TValueType>(TValueType value)
        {
            _values ??= new Dictionary<Type, IValidationValue>();
            var validationValue = new ValidationValue<TValueType>(value);
            if (!_values.TryAdd(typeof(TValueType), validationValue))
                _values[typeof(TValueType)] = validationValue;
            return this;
        }

        /// <summary>
        /// Create a Passing ValidationResult
        /// </summary>
        /// <returns></returns>
        public static ValidationResult Pass() =>
            new ValidationResultWithSourceAndRule(true, string.Empty);

        /// <summary>
        /// Create a Failing ValidationResult with an optional message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ValidationResult Fail(string errorMessage = "") =>
            new ValidationResultWithSourceAndRule(false, errorMessage);

        /// <summary>
        ///  Set a ValidationResult as passed or failed. Optionally access an error message. The error message is discarded if the passed parameter is true.
        /// </summary>
        /// <param name="passed"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ValidationResult Set(bool passed, string errorMessage = "") =>
            new ValidationResultWithSourceAndRule(passed, passed ? string.Empty : errorMessage);
    }

    public class ValidationResultWithSourceAndRule : ValidationResult
    {
        public ValidationResultWithSourceAndRule(bool passed, string errorMessage = "")
            : base(passed, errorMessage)
        {
        }

        public ValidationResultWithSourceAndRule SetSourceAndRuleDetails<TSource, TValidationRule>()
        {
            Source = $"{typeof(TSource)}";
            Rule = $"{typeof(TValidationRule)}";
            return this;
        }

        public static ValidationResultWithSourceAndRule Create<TSource, TValidationRule>(bool passed, string errorMessage = "")
        {
            var result = new ValidationResultWithSourceAndRule(passed, errorMessage)
                .SetSourceAndRuleDetails<TSource, TValidationRule>();
            return result;
        }
    }

}
