namespace Crank.Validation
{
    public interface IValidationRule
    {
    }
    public interface IValidationRule<TSource> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source);
    }

    public interface IValidationRule<TSource, TValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, TValue value);
    }

    public interface IValidationRule<TSource, TValue, TAdditionalValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, TValue value, TAdditionalValue additionalValue);
    }

    public interface IValidationRuleWithOutValue<TSource, TOutValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, out TOutValue outValue);
    }

    public interface IValidationRuleWithOutValues<TSource, TOutValue, TAdditionalOutValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, out TOutValue outValue, out TAdditionalOutValue additionalOutValue);
    }
}
