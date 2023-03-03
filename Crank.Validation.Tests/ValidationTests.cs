using Crank.Validation.Tests.Models;
using Crank.Validation.Tests.Validations;
using System;
using Xunit;

namespace Crank.Validation.Tests
{
    public class ValidationTests
    {
        private static Validation CreateValidation()
        {
            var validation = new Validation(
                new IValidationRule[]
                {
                    new CheckThatCustomerNamesAreValid(),
                    new CheckThatTheCustomerHasAValidAddress(),
                });
            return validation;
        }

        [Fact]
        public void WhenTestingASingleRule_IfTheRulePasses_TheValidationShouldPass()
        {
            //given
            var validation = CreateValidation();
            var customer = new CustomerModel()
            {
                FirstName = "firstname",
                LastName = "lastname"
            };

            //when
            var validationResult = validation.For(customer)
                .ApplyRule<CheckThatCustomerNamesAreValid>();

            var validationRuleResult =
                validationResult.Result<CheckThatCustomerNamesAreValid>();

            //then
            Assert.NotNull(validationResult);
            Assert.True(validationResult.Passing);
            Assert.NotNull(validationRuleResult);
            Assert.True(validationRuleResult.Passed);
            Assert.Empty(validationResult.Failures);
        }

        [Fact]
        public void WhenTestingASingleRule_IfTheRuleFails_TheValidationShouldPass()
        {
            //given
            var validation = CreateValidation();
            var customer = new CustomerModel()
            {
                FirstName = "firstname",

            };

            //when
            var validationResult = validation.For(customer)
                .ApplyRule<CheckThatCustomerNamesAreValid>();

            var validationRuleResult =
                validationResult.Result<CheckThatCustomerNamesAreValid>();

            //then
            Assert.NotNull(validationResult);
            Assert.False(validationResult.Passing);
            Assert.NotNull(validationRuleResult);
            Assert.False(validationRuleResult.Passed);
            Assert.NotEmpty(validationResult.Failures);
        }

        [Fact]
        public void WhenTestingMultipleRules_IfTheRulesPass_TheValidationShouldPass()
        {
            //given
            var validation = CreateValidation();
            var customer = new CustomerModel()
            {
                FirstName = "firstname",
                LastName = "lastname",
                Address = new AddressModel()
                {
                    Street = "123 south street",
                    City = "lost city",
                    State = "atlantis",
                    ZipCode = "1234"
                }
            };

            //when
            var validationResult = validation.For(customer)
                .ApplyRule<CheckThatCustomerNamesAreValid>()
                .ApplyRule<CheckThatTheCustomerHasAValidAddress>();

            var customerNameRuleResult =
                validationResult.Result<CheckThatCustomerNamesAreValid>();
            var addressValidationRuleResult =
                validationResult.Result<CheckThatTheCustomerHasAValidAddress>();

            //then
            Assert.NotNull(validationResult);
            Assert.True(validationResult.Passing);
            Assert.NotNull(customerNameRuleResult);
            Assert.True(customerNameRuleResult.Passed);
            Assert.NotNull(addressValidationRuleResult);
            Assert.True(addressValidationRuleResult.Passed);
            Assert.Empty(validationResult.Failures);
        }
    }
}
