namespace NorthWind.ValidationService.FluentValidation;

internal class ValidationRules<T, TProperty>(
    IRuleBuilderInitial<T, TProperty> ruleBuilderInitial) :
    IValidationRules<T, TProperty>
{
    // Utilizada para reglas sobre propiedades de tipo string
    IRuleBuilder<T, string> StringRuleBuilder =>
        (IRuleBuilder<T, string>)ruleBuilderInitial;

    // Utilizada para devolver el resultado de una regla en
    // propiedades de tipo string.
    IValidationRules<T, string> ThisAsStringValidationRules =>
        (IValidationRules<T, string>)this;

    public IValidationRules<T, TProperty> NotEmpty(string errorMessage)
    {
        ruleBuilderInitial.NotEmpty()
            .WithMessage(errorMessage);

        return this;
    }

    public IValidationRules<T, TProperty> NotNull(string errorMessage)
    {
        ruleBuilderInitial
            .NotNull()
            .WithMessage(errorMessage);

        return this;
    }

    public IValidationRules<T, TProperty> Must(Func<TProperty, bool> predicate,
        string errorMessage)
    {
        ruleBuilderInitial
            .Must(predicate)
            .WithMessage(errorMessage);

        return this;
    }

    public IValidationRules<T, TProperty> StopOnFirstFailure()
    {
        ruleBuilderInitial
            .Cascade(CascadeMode.Stop);

        return this;
    }

    public IValidationRules<T, TProperty> Equal(
        Expression<Func<T, TProperty>> expression, string errorMessage)
    {
        ruleBuilderInitial
            .Equal(expression)
            .WithMessage(errorMessage);

        return this;
    }

    public IValidationRules<T, TProperty> GreaterThan<TValue>(
        TValue valueToCompare, string errorMessage)
        where TValue : TProperty, IComparable<TValue>, IComparable
    {
        IRuleBuilder<T, TValue> Builder =
            (IRuleBuilder<T, TValue>)ruleBuilderInitial;

        Builder
            .GreaterThan(valueToCompare)
            .WithMessage(errorMessage);

        return this;
    }

    public IValidationRules<T, string> Length(
        int length, string errorMessage)
    {
        StringRuleBuilder
            .Length(length)
            .WithMessage(errorMessage);

        return ThisAsStringValidationRules;
    }

    public IValidationRules<T, string> MaximumLength(
        int length, string errorMessage)
    {
        StringRuleBuilder
            .MaximumLength(length)
            .WithMessage(errorMessage);

        return ThisAsStringValidationRules;
    }

    public IValidationRules<T, string> MinimumLength(
        int length, string errorMessage)
    {
        StringRuleBuilder
            .MinimumLength(length)
            .WithMessage(errorMessage);

        return ThisAsStringValidationRules;
    }

    public IValidationRules<T, string> EmailAddress(string errorMessage)
    {
        StringRuleBuilder
            .EmailAddress()
            .WithMessage(errorMessage);

        return ThisAsStringValidationRules;
    }

    public IValidationRules<T, string> Matches(string pattern,
        string errorMessage)
    {
        StringRuleBuilder
            .Matches(pattern)
            .WithMessage(errorMessage);

        return ThisAsStringValidationRules;
    }
}
