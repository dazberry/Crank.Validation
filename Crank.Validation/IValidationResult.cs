namespace Crank.Validation
{
    public interface IValidationResult
    {
        /// <summary>
        /// Qualified Source name
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Qualified Rule name
        /// </summary>
        string Rule { get; }

        /// <summary>
        /// Check that the associated rule has passed
        /// </summary>
        bool Passed { get; }
        string ErrorMessage { get; }

        /// <summary>
        /// Retrieve a value by type from the internal values dictionary
        /// </summary>
        /// <typeparam name="TValueType"></typeparam>
        /// <param name="value"></param>
        /// <returns>bool</returns>
        bool TryGetValue<TValueType>(out TValueType value);

                /// <summary>
        ///  Adds or replaces a validation Value
        /// </summary>
        /// <typeparam name="TValueType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        IValidationResult WithValue<TValueType>(TValueType value);
    }

}
