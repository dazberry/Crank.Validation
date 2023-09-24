using System.Threading.Tasks;

namespace Crank.Validation
{
    public interface IValidationRule
    {
    }

    /// <summary>
    /// Validation Rule against a Source value
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IValidationRule<TSource> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source);
    }

    /// <summary>
    /// Validation Rule with an input value against a Source value
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TInputValue"></typeparam>
    public interface IValidationRule<TSource, TInputValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, TInputValue inputValue);
    }

    /// <summary>
    /// Validation Rule with two input values against a Source value
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TInputValue"></typeparam>
    /// <typeparam name="TAdditionalInputValue"></typeparam>
    public interface IValidationRule<TSource, TInputValue, TAdditionalInputValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, TInputValue inputValue, TAdditionalInputValue additionalValue);
    }

    /// <summary>
    /// Validation Rule against a Source value that returns an output value
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOutValue"></typeparam>
    public interface IValidationRuleWithOutValue<TSource, TOutValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, out TOutValue outValue);
    }

    /// <summary>
    /// Validation Rule against a Source value that returns two output values
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOutValue"></typeparam>
    /// <typeparam name="TAdditionalOutValue"></typeparam>
    public interface IValidationRuleWithOutValues<TSource, TOutValue, TAdditionalOutValue> : IValidationRule
    {
        public IValidationResult ApplyTo(TSource source, out TOutValue outValue, out TAdditionalOutValue additionalOutValue);
    }

    public interface IValidationRuleAsync : IValidationRule
    {

    }

    /// <summary>
    /// A Validation Rule against a Source that returns Task IValidationResult
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IValidationRuleAsync<TSource> : IValidationRuleAsync
    {
        public Task<IValidationResult> ApplyTo(TSource source);
    }

    /// <summary>
    /// A Validation Rule against a Source that takes an input parameter and returns Task IValidationResult
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TInputValue"></typeparam>
    public interface IValidationRuleAsync<TSource, TInputValue> : IValidationRuleAsync
    {
        public Task<IValidationResult> ApplyTo(TSource source, TInputValue inputValue);
    }

    /// <summary>
    /// A Validation Rule against a Source that takes two input parameter and returns Task IValidationResult
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TInputValue"></typeparam>
    public interface IValidationRuleAsync<TSource, TInputValue, TAdditionalInputValue> : IValidationRuleAsync
    {
        public Task<IValidationResult> ApplyTo(TSource source, TInputValue inputValue, TAdditionalInputValue additionalValue);

    }
}
