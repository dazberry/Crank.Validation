using System.Threading.Tasks;

namespace Crank.Validation
{
    public interface IValidationRule
    {
    }

    public interface IValidationRule<TSource> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source);
    }

    public interface IValidationRule<TSource, TInputValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, TInputValue inputValue);
    }

    public interface IValidationRule<TSource, TInputValue, TAdditionalInputValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, TInputValue inputValue, TAdditionalInputValue additionalValue);
    }

    public interface IValidationRuleWithOutValue<TSource, TOutValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, out TOutValue outValue);
    }

    public interface IValidationRuleWithOutValues<TSource, TOutValue, TAdditionalOutValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, out TOutValue outValue, out TAdditionalOutValue additionalOutValue);
    }

    public interface IValidationRuleAsync : IValidationRule
    {

    }

    public interface IValidationRuleAsync<TSource> : IValidationRuleAsync
    {
        public Task<IValidationResult> ApplyTo(TSource source);
    }

    public interface IValidationRuleAsync<TSource, TInputValue> : IValidationRuleAsync
    {
        public Task<IValidationResult> ApplyTo(TSource source, TInputValue inputValue);
    }

    public interface IValidationRuleAsync<TSource, TInputValue, TAdditionalInputValue> : IValidationRuleAsync
    {
        public Task<IValidationResult> ApplyTo(TSource source, TInputValue inputValue, TAdditionalInputValue additionalValue);

    }
}
