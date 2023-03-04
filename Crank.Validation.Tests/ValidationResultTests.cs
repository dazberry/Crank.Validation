using Crank.Validation.Tests.Models;
using Crank.Validation.Tests.Validations;
using Xunit;

namespace Crank.Validation.Tests.Validations
{
    public class ValidationResultTests
    {

        [Fact]
        public void WhenPassingAValidationResult_TheValidationResult_ShouldBeFlaggedAsPassing()
        {
            //when
            var validationResult = ValidationResult.Pass();

            //then
            Assert.True(validationResult.Passed);
        }

        [Fact]
        public void WhenFailingAValidationResult_TheValidationResult_ShouldBeFlaggedAsFailing()
        {
            //when
            var validationResult = ValidationResult.Fail();

            //then
            Assert.False(validationResult.Passed);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenSettingAValidationResult_TheValidationResult_ShouldBeFlaggedAsPerTheSetValue(bool passing)
        {
            //when
            var validationResult = ValidationResult.Set(passing);

            //then
            Assert.True(validationResult.Passed == passing);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenSettingAValidationResult_TheErrorMessage_IsOnlyPassedWhenNotPassing(bool passing)
        {
            //give
            var errorMessage = "Error Message";

            //when
            var validationResult = ValidationResult.Set(passing, errorMessage);

            //then
            Assert.True(validationResult.Passed == passing);
            Assert.Equal(passing, string.IsNullOrEmpty(validationResult.ErrorMessage));
        }

    }
}
