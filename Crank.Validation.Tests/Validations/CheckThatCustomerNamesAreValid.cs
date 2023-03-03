using Crank.Validation.Tests.Models;
using System.Linq;

namespace Crank.Validation.Tests.Validations
{
    public class CheckThatCustomerNamesAreValid : IValidationRule<CustomerModel>
    {

        private readonly string[] Titles = new[] { "Mr", "Ms", "Mrs", "Miss", "Dr" };

        public IValidationResult ApplyTo(CustomerModel source)
        {
            if (string.IsNullOrEmpty(source.FirstName))
                return ValidationResult.Fail($"{nameof(source.FirstName)} not specified");

            if (string.IsNullOrEmpty(source.LastName))
                return ValidationResult.Fail($"{nameof(source.LastName)} not specified");

            if (!string.IsNullOrEmpty(source.Title) && !Titles.Any(t => string.Equals(t, source.Title)))
                return ValidationResult.Fail($"{nameof(source.Title)} value is invalid");

            return ValidationResult.Pass();
        }
    }
}
