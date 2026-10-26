namespace NorthWind.Membership.Entities.Validators.UserLogin;

internal class UserCredentialsDtoValidator :
    AbstractModelValidator<UserCredentialsDto>
{
    public UserCredentialsDtoValidator(
        IValidationService<UserCredentialsDto> validationService) :
        base(validationService)
    {
        AddRuleFor(u => u.Email)
            .StopOnFirstFailure()
            .NotEmpty(UserLoginMessages.RequiredEmailErrorMessage)
            .MaximumLength(UserRegistrationDtoValidator.EmailMaximumLength,
                UserLoginMessages.EmailMaximumLengthErrorMessage)
            .Matches(UserRegistrationDtoValidator.EmailPattern,
                UserLoginMessages.InvalidEmailCharactersErrorMessage);

        AddRuleFor(u => u.Password)
            .NotEmpty(UserLoginMessages.RequiredPasswordErrorMessage);
    }
}
