using Crank.Validation.Tests.Models;
using Crank.Validation.Tests.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Crank.Validation.Tests
{
    public class ValidationTests
    {
        private static Validation CreateValidation(ValidationOptions validationOptions = null)
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
                    new ARuleThatComparesAgainstAGuidValue(),
                },
                validationOptions);
            return validation;
        }

        private static void AssertHasPassed<TValidationSource, TValidationRule>(ValidationSource validationSource)
        {
            var passingResult = validationSource.Result<TValidationRule>();
            Assert.NotNull(passingResult);
            Assert.Equal($"{typeof(TValidationSource)}", passingResult.Source);
            Assert.Equal($"{typeof(TValidationRule)}", passingResult.Rule);
            Assert.True(passingResult.Passed);
            Assert.True(validationSource.Passed<TValidationRule>());
        }

        private static void AssertHasFailed<TValidationSource, TValidationRule>(ValidationSource validationSource)
        {
            var failingResult = validationSource.Result<TValidationRule>();
            Assert.NotNull(failingResult);
            Assert.Equal($"{typeof(TValidationSource)}", failingResult.Source);
            Assert.Equal($"{typeof(TValidationRule)}", failingResult.Rule);
            Assert.False(failingResult.Passed);
            Assert.Contains(failingResult, validationSource.Failures);
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
            AssertHasPassed<SourceModel, ARuleThatPassesAndReturnsAStringValue>(validationSource);
            AssertHasFailed<SourceModel, ARuleThatFailsWithAnErrorMessage>(validationSource);
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
            AssertHasPassed<SourceModel, ARuleThatPassesAndReturnsAStringValueAsync>(validationSource);
            AssertHasFailed<SourceModel, ARuleThatFailsWithAnErrorMessageAsync>(validationSource);
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
            AssertHasPassed<SourceModel, ARuleThatPassesAndReturnsAStringValue>(validationSource);
            AssertHasFailed<SourceModel, ARuleThatFailsWithAnErrorMessage>(validationSource);
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IfStopApplyingRulesAfterFailure_AndARuleFails_NoAdditionalRulesShouldBeCalled(bool flagValue)
        {
            //given
            var validation = CreateValidation(new ValidationOptions() { StopApplyingRulesAfterFailure = flagValue });
            var sourceModel = new SourceModel();
            var expectedText = flagValue ? "Rule not applied - due to previous failing validations" : string.Empty;

            //when
            validation.For(sourceModel)
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>(out IValidationResult failingRuleResult)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>(out IValidationResult passingRuleResult);

            //then
            Assert.False(failingRuleResult.Passed);
            Assert.Equal(!flagValue, passingRuleResult.Passed);
            Assert.Equal(expectedText, passingRuleResult.ErrorMessage);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ForOptionsOverrideValidationOptions_AndARuleFails_NoAdditionalRulesShouldBeCalled(bool flagValue)
        {
            //given
            var validation = CreateValidation(new ValidationOptions() { StopApplyingRulesAfterFailure = !flagValue });
            var sourceModel = new SourceModel();
            var expectedText = flagValue ? "Rule not applied - due to previous failing validations" : string.Empty;

            //when
            validation.For(sourceModel, opt => opt.StopApplyingRulesAfterFailure = flagValue)
                .ApplyRule<ARuleThatFailsWithAnErrorMessage>(out IValidationResult failingRuleResult)
                .ApplyRule<ARuleThatPassesAndReturnsAStringValue>(out IValidationResult passingRuleResult);

            //then
            Assert.False(failingRuleResult.Passed);
            Assert.Equal(!flagValue, passingRuleResult.Passed);
            Assert.Equal(expectedText, passingRuleResult.ErrorMessage);
        }

        [Fact]
        public void WhenChainingFor_IfAllRulesPassForAllSources_TheValidationShouldPass()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };
            var guidValue = Guid.NewGuid();
            var anOtherSource = new AnOtherSourceModel { AGuidValue = guidValue };

            //when
            //when
            var validationSource = validation
                .For(source)
                    .ApplyRule<ARuleThatPassesAndReturnsAStringValue>()
                .For(anOtherSource)
                    .ApplyRule<ARuleThatComparesAgainstAGuidValue, Guid>(guidValue);
            var passing = validationSource.Passing;


            //then
            Assert.True(passing);
            Assert.Equal(2, validationSource.Results.Count());
            AssertHasPassed<SourceModel, ARuleThatPassesAndReturnsAStringValue>(validationSource);
            AssertHasPassed<AnOtherSourceModel, ARuleThatComparesAgainstAGuidValue>(validationSource);
        }

        [Fact]
        public void WhenChainingFor_IfFirstSourceRuleFails_TheValidationShouldFail()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };
            var guidValue = Guid.NewGuid();
            var anOtherSource = new AnOtherSourceModel { AGuidValue = guidValue };

            //when
            var validationSource = validation
                .For(source)
                    .ApplyRule<ARuleThatFailsWithAnErrorMessage>()
                .For(anOtherSource)
                    .ApplyRule<ARuleThatComparesAgainstAGuidValue, Guid>(guidValue);
            var passing = validationSource.Passing;

            //then
            Assert.False(passing);
            AssertHasFailed<SourceModel, ARuleThatFailsWithAnErrorMessage>(validationSource);
            AssertHasPassed<AnOtherSourceModel, ARuleThatComparesAgainstAGuidValue>(validationSource);
        }


        [Fact]
        public void WhenChainingFor_IfSecondSourceRuleFails_TheValidiationShouldFail()
        {
            //given
            var validation = CreateValidation();
            var source = new SourceModel() { AStringValue = "123" };
            var guidValue = Guid.NewGuid();
            var anOtherSource = new AnOtherSourceModel { AGuidValue = guidValue };

            //when
            var validationSource = validation
                .For(anOtherSource)
                    .ApplyRule<ARuleThatComparesAgainstAGuidValue, Guid>(guidValue)
                .For(source)
                    .ApplyRule<ARuleThatFailsWithAnErrorMessage>();

            var passing = validationSource.Passing;

            //then
            Assert.False(passing);
            AssertHasPassed<AnOtherSourceModel, ARuleThatComparesAgainstAGuidValue>(validationSource);
            AssertHasFailed<SourceModel, ARuleThatFailsWithAnErrorMessage>(validationSource);
        }
    }


}
