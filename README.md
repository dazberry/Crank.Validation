
# Crank.Validation

**crank**

*verb (used without object)*
to turn a crank, as in starting an automobile engine.

*noun*
Informal. an ill-tempered, grouchy person.

---

**What**

Crank.Validation; in a similar vein to Crank.Mapper, is a set of classes designed to allow validation symantics against a specific source object. As with Crank.Mapper, the actually validations need to be written manually.

**Why**

While using Crank.Mapper in addition to a self-rolled mediation library, there seemed to be a gap in relation to validating specific input data. This is the result trying to plug that gap.

## **Validation**

When constructing a Validation instance,  a collection of validation rules should be registered with that instance. 
When invoking the For and ApplyRule functions, the engine will search for a match based on the source and rule types. If a match is found it will invoke the matching implementation.

    Validation.For(aSourceValue)
        .ApplyRule<RuleType>()
        .ApplyRule<RuleType>(out IValidationResult resultOfThisRule)
        .ApplyRule<RuleType, inputType>(anInputValue)
        .ApplyRule<RuleType, inputType>(anInputValue, out IValidationResult resultOfThisRule)
        .ApplyRule<RuleType, inputType, additionalValueType>(additionalInputType>(anInputValue, anAdditionalValue)
        .ApplyRule<RuleType, inputType, additionalValueType>(additionalInputType>(anInputValue, anAdditionalValue, out IValidationResult resultOfThisRule);	   


## **IValidationRule`<SourceModel>`**

The most basic validation rule works against the source value without any additional input data. The ApplyTo method should be implemented and return either a passing or failing **IValidationResult**.

    public class StringHasValueButNotMoreThan100Characters<string> : IValidationRule<string>
    {
        public IValidationResult ApplyTo(string source)
        {
            if (string.IsNullOrEmpty(source))
                return ValidationResult.Fail("Value is null");
            if (source.Length > 100)
                return ValidationResult.Fail("Value is too long");
            return ValidationResult.Pass();
        }
    }

## **IValidationRule`<SourceModel, InputValue>`**
Additionally a value can be passed to the validation rule depending on how the rule is specified.

    public class SourceValueMatchesInputValue<string, string> : IValidationRule<string>
    {
        public IValidationResult ApplyTo(string source, string inputValue)
        {
            if (!string.Equals(source, inputValue))
                return ValidationResult.Fail("Values do not match");
            return ValidationResult.Pass();
        }
    }

## Invoking validation rules
Single Rule validation

    var inputValue = "Hello World";
    var validation = new Validation(listOfRules); 
    var passing = validation.For(inputValue)
        .ApplyRule<StringHasValueButNotMoreThan100Characters>()
        .Passing
       
Multiple Rule validation

    var inputValue = "Hello Earth";
    var compareTo = "Hello Mars";
    var validation = new Validation(listOfRules); 
    var passing = validation.For(inputValue)
        .ApplyRule<StringHasValueButNotMoreThan100Characters>()
        .ApplyRule<SourceValueMatchesInputValue>(compareTo)
        .Passing

Validation.For(...) returns a **ValidationSource**. Each call to the various ApplyRule methods continue returning ValidationSource values. Passing then returns a boolean value.

## Getting the result of Applied Rules from a ValidationSource
|Method|Returns  |
|--|--|
|Passing  | All rules passing return true  |
|  | Any rules failing return false |
|  | If no rules returns true
|Passed<`TRuleType`>() | If a matching rule exists return if the rule passed |
|  |If no matching rule exists, returns false|
|Results| A list of all validation rule results
|Result<`TRuleType`>()|If a matching rule exists, returns the validation result or default|
|Failures | A list of all validation rules that have failed|
|Reset() | Empty the list of ValidationResults in the ValidationSource|
