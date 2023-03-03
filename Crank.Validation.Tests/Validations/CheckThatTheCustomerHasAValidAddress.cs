using Crank.Validation.Tests.Models;

namespace Crank.Validation.Tests.Validations
{
    public class CheckThatTheCustomerHasAValidAddress : IValidationRule<CustomerModel>
    {
        public IValidationResult ApplyTo(CustomerModel source)
        {
            if (source == null)
                return ValidationResult.Fail("CustomerModel not specfied");

            if (source.Address == null)
                return ValidationResult.Fail("Customer Address not specfied");

            if (string.IsNullOrEmpty(source.Address.Street) ||
                string.IsNullOrEmpty(source.Address.City) ||
                string.IsNullOrEmpty(source.Address.State) ||
                string.IsNullOrEmpty(source.Address.ZipCode))
                return ValidationResult.Fail("Address is incomplete");

            return ValidationResult.Pass();
        }
    }
}
