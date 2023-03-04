using Crank.Validation.Tests.Models;
using Crank.Validation.Tests.Validations;
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
                    new ARuleThatPassesAndReturnsAStringValue(),
                    new ARuleThatFailsWithAnErrorMessage(),
                    new ARuleThatChecksTheSourceStringValue(),

                });
            return validation;
        }


        [Fact]
        public void WhenARulePasses_ValidationPassed_ShouldBeTrue()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var passing = validation.For(source)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>()
                .Passing;

            //then
            Assert.True(passing);
        }

        [Fact]
        public void WhenARulePasses_AMatchingValidationResultShouldExist_AndHavePassed()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>();
            var validationResult = validationSource.Result<ARuleThatPassesAndReturnsAStringValue>();

            //then
            Assert.NotNull(validationResult);
            Assert.True(validationResult.Passed);
        }

        [Fact]
        public void WhenARuleFails_ValidationPassed_ShouldBeFalse()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var passing = validation.For(source)
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>()
                .Passing;

            //then
            Assert.False(passing);
        }

        [Fact]
        public void WhenARuleFails_AMatchingValidationResultShouldExist_AndNotPassed()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>();
            var validationResult = validationSource.Result<ARuleThatFailsWithAnErrorMessage>();

            //then
            Assert.NotNull(validationResult);
            Assert.False(validationResult.Passed);
        }

        [Fact]
        public void WhenMultipleRuleAreInvoked_IfOneFails_TheValidationShouldFail()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>()
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>();

            //then
            Assert.False(validationSource.Passing);
            Assert.NotEmpty(validationSource.Failures);
            Assert.NotNull(validationSource.Result<ARuleThatPassesAndReturnsAStringValue>());
            Assert.NotNull(validationSource.Result<ARuleThatFailsWithAnErrorMessage>());
            Assert.True(validationSource.Result<ARuleThatPassesAndReturnsAStringValue>().Passed);
            Assert.True(validationSource.Passed<ARuleThatPassesAndReturnsAStringValue>());
            Assert.False(validationSource.Result<ARuleThatFailsWithAnErrorMessage>().Passed);
            Assert.False(validationSource.Passed<ARuleThatFailsWithAnErrorMessage>());
        }

        [Fact]
        public void IfARuleSpecifiesWithValue_ThatValueShouldBeAvailable_InTheValidationResult()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>();

            var tryGetValue = validationSource.TryGetValue<ARuleThatPassesAndReturnsAStringValue, string>(out var returnedValue);
            var validationRule = validationSource.Result<ARuleThatPassesAndReturnsAStringValue>();
            var tryGetValue2 = validationRule.TryGetValue<string>(out var returnedValue2);

            //then
            Assert.True(tryGetValue);
            Assert.Equal(source.AStringValue, returnedValue);
            Assert.NotNull(validationRule);
            Assert.True(tryGetValue2);
            Assert.Equal(source.AStringValue, returnedValue2);
        }

        [Fact]
        public void IfARuleSpecifiesFails_AndSpecifiesAnErrorMessage_TheMessageShouldBeInTheValidationResult()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>();

            //then
            Assert.NotNull(validationSource.Result<ARuleThatFailsWithAnErrorMessage>()?.ErrorMessage);
            Assert.Equal(validationSource.Result<ARuleThatFailsWithAnErrorMessage>()?.ErrorMessage, source.AStringValue);
        }
    }
}
