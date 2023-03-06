using Crank.Validation.Tests.Models;
using Crank.Validation.Tests.Validations;
using System.Linq;
using System.Threading.Tasks;
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
                    new ARuleThatPassesAndReturnsAStringValueAsync(),
                    new ARuleThatFailsWithAnErrorMessage(),
                    new ARuleThatFailsWithAnErrorMessageAsync(),
                    new ARuleThatComparesSourceAgainstAnInputValue(),
                    new ARuleThatUpdatesAnInputValue(),
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
        public async Task WhenAnAsyncRulePasses_ValidationPassed_ShouldBeTrue()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var passing = (await validation.For(source)
                .ApplyRuleAsync<ARuleThatPassesAndReturnsAStringValueAsync>())
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
        public async Task WhenAnAsyncRulePasses_AMatchingValidationResultShouldExist_AndHavePassed()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = await validation.For(source)
                .ApplyRuleAsync<ARuleThatPassesAndReturnsAStringValueAsync>();
            var validationResult = validationSource.Result<ARuleThatPassesAndReturnsAStringValueAsync>();

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
        public async Task WhenAnAsyncRuleFails_ValidationPassed_ShouldBeFalse()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var passing = (await validation.For(source)
                .ApplyRuleAsync<ARuleThatFailsWithAnErrorMessageAsync>())
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
        public async Task WhenAnAsyncRuleFails_AMatchingValidationResultShouldExist_AndNotPassed()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = await validation.For(source)
                .ApplyRuleAsync<ARuleThatFailsWithAnErrorMessageAsync>();
            var validationResult = validationSource.Result<ARuleThatFailsWithAnErrorMessageAsync>();

            //then
            Assert.NotNull(validationResult);
            Assert.False(validationResult.Passed);
        }

        [Fact]
        public void WhenApplyingARule_ThatIsNotRegistered_TheValidationShouldFail()
        {
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ThisRuleIsNeverRegistered>();
            var validationResult = validationSource.Result<ThisRuleIsNeverRegistered>();
            var validationResult2 = validationSource.Failures.FirstOrDefault();

            //then
            Assert.False(validationSource.Passing);
            Assert.Equal("Rule not found", validationResult.ErrorMessage);
            Assert.Equal(validationResult, validationResult2);
        }


        [Fact]
        public void WhenMultipleRulesAreInvoked_IfOneFails_TheValidationShouldFail()
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
        public async Task WhenMultipleAsyncRulesAreInvoked_IfOneFails_TheValidationShouldFail()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel { AStringValue = "123" };

            //when
            var validationSource = await (await validation.For(source)
                .ApplyRuleAsync<ARuleThatPassesAndReturnsAStringValueAsync>())
                .ApplyRuleAsync<ARuleThatFailsWithAnErrorMessageAsync>();

            //then
            Assert.False(validationSource.Passing);
            Assert.NotEmpty(validationSource.Failures);
            Assert.NotNull(validationSource.Result<ARuleThatPassesAndReturnsAStringValueAsync>());
            Assert.NotNull(validationSource.Result<ARuleThatFailsWithAnErrorMessageAsync>());
            Assert.True(validationSource.Result<ARuleThatPassesAndReturnsAStringValueAsync>().Passed);
            Assert.True(validationSource.Passed<ARuleThatPassesAndReturnsAStringValueAsync>());
            Assert.False(validationSource.Result<ARuleThatFailsWithAnErrorMessageAsync>().Passed);
            Assert.False(validationSource.Passed<ARuleThatFailsWithAnErrorMessageAsync>());
        }

        [Fact]
        public void WhenInvokingRules_TheValidationResult_CanBeReturnedFromTheApplyRuleMethod()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel { AStringValue = "123" };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>(out var passingRuleResult)
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>(out var failingRuleResult);

            //then
            Assert.False(validationSource.Passing);
            Assert.NotEmpty(validationSource.Failures);
            Assert.NotNull(passingRuleResult);
            Assert.NotNull(failingRuleResult);
            Assert.True(passingRuleResult.Passed);
            Assert.True(validationSource.Result<ARuleThatPassesAndReturnsAStringValue>().Passed);
            Assert.True(validationSource.Passed<ARuleThatPassesAndReturnsAStringValue>());
            Assert.False(failingRuleResult.Passed);
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

        [Fact]
        public void IfARuleTakesAnInputValue_ThatInputValue_ShouldBeAvailableToTheRule()
        {
            //given
            var stringValue = "123";
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = stringValue };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatComparesSourceAgainstAnInputValue, string>(stringValue);

            //then
            Assert.True(validationSource.Passing);
            Assert.True(validationSource.TryGetValue(out string inputValue));
            Assert.Equal(stringValue, inputValue);
        }

        [Fact]
        public void IfARuleUpdatesAnInputValue_ComparingTheInputValue_ShouldMatchTheSourceValue()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel()
            {
                AStringValue = "123",
                AnIntegerValue = 456
            };
            var inputValue = new SourceModel()
            {
                AStringValue = "discard",
                AnIntegerValue = -1
            };

            //when
            var validationSource = validation.For(source)
                .ApplyRule<ARuleThatUpdatesAnInputValue, SourceModel>(inputValue);

            //then
            Assert.Equal(source.AnIntegerValue, inputValue.AnIntegerValue);
            Assert.Equal(source.AStringValue, inputValue.AStringValue);
            Assert.True(validationSource.TryGetValue(out SourceModel inputValue2));
            Assert.Equal(inputValue, inputValue2);
        }


    }
}
