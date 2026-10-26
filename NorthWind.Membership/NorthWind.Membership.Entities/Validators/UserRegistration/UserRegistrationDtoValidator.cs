namespace NorthWind.Membership.Entities.Validators.UserRegistration;

internal class UserRegistrationDtoValidator :
    AbstractModelValidator<UserRegistrationDto>
{
    internal const int NameMaximumLength = 25;
    internal const int EmailMaximumLength = 50;
    internal const string NamePattern =
        @"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$";
    internal const string EmailPattern =
        @"^[a-zA-Z0-9._]+@([a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$";

    public UserRegistrationDtoValidator(
        IValidationService<UserRegistrationDto> validationService) :
        base(validationService)
    {
        AddRuleFor(u => u.FirstName)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredFirstNameErrorMessage)
            .MaximumLength(NameMaximumLength,
                UserRegistrationMessages.FirstNameMaximumLengthErrorMessage)
            .Matches(NamePattern,
                UserRegistrationMessages.FirstNameInvalidFormatErrorMessage);

        AddRuleFor(u => u.LastName)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredLastNameErrorMessage)
            .MaximumLength(NameMaximumLength,
                UserRegistrationMessages.LastNameMaximumLengthErrorMessage)
            .Matches(NamePattern,
                UserRegistrationMessages.LastNameInvalidFormatErrorMessage);

        AddRuleFor(u => u.Email)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredEmailErrorMessage)
            .MaximumLength(EmailMaximumLength,
                UserRegistrationMessages.EmailMaximumLengthErrorMessage)
            .Matches(EmailPattern,
                UserRegistrationMessages.InvalidEmailCharactersErrorMessage);

        AddRuleFor(u => u.Password)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredPasswordErrorMessage)
            .MinimumLength(6,
                UserRegistrationMessages.PasswordTooShortErrorMessage)
            .Must(p => p.Any(c => char.IsLower(c)),
                UserRegistrationMessages.PasswordRequiresLowerErrorMessage)
            .Must(p => p.Any(c => char.IsUpper(c)),
                UserRegistrationMessages.PasswordRequiresUpperErrorMessage)
            .Must(p => p.Any(c => char.IsDigit(c)),
                UserRegistrationMessages.PasswordRequiresDigitErrorMessage)
            .Must(p => p.Any(c => !char.IsLetterOrDigit(c)),
                UserRegistrationMessages
                    .PasswordRequiresNonAlphanumericErrorMessage);
    }
}
