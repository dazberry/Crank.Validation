using System;
using System.Collections.Generic;

namespace Crank.Validation
{
    public class ValidationResult : IValidationResult
    {
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

        public ValidationResult(bool passed, string errorMessage = "")
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
            _values.Add(typeof(TValueType), new ValidationValue<TValueType>(value));
            return this;
        }

        public static ValidationResult Pass() =>
            new ValidationResult(true, string.Empty);

        public static ValidationResult Fail(string errorMessage = "") =>
            new ValidationResult(false, errorMessage);

        public static ValidationResult Set(bool passed, string errorMessage = "") =>
            new ValidationResult(passed, passed ? string.Empty : errorMessage);
    }

}
