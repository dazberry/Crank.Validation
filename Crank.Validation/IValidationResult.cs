namespace Crank.Validation
{
    public interface IValidationResult
    {
        bool Passed { get; }
        string ErrorMessage { get; }

        bool TryGetValue<TValueType>(out TValueType value);
        IValidationResult WithValue<TValueType>(TValueType value);
    }

}
