#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Membership.Entities.Resources;

public static class UserRegistrationMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Membership.Entities.Resources.UserRegistrationMessages",
        typeof(UserRegistrationMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string RequiredFirstNameErrorMessage =>
        ResourceManager.GetString(nameof(RequiredFirstNameErrorMessage), Culture)!;
    public static string FirstNameMaximumLengthErrorMessage =>
        ResourceManager.GetString(nameof(FirstNameMaximumLengthErrorMessage), Culture)!;
    public static string FirstNameInvalidFormatErrorMessage =>
        ResourceManager.GetString(nameof(FirstNameInvalidFormatErrorMessage), Culture)!;
    public static string RequiredLastNameErrorMessage =>
        ResourceManager.GetString(nameof(RequiredLastNameErrorMessage), Culture)!;
    public static string LastNameMaximumLengthErrorMessage =>
        ResourceManager.GetString(nameof(LastNameMaximumLengthErrorMessage), Culture)!;
    public static string LastNameInvalidFormatErrorMessage =>
        ResourceManager.GetString(nameof(LastNameInvalidFormatErrorMessage), Culture)!;
    public static string RequiredEmailErrorMessage =>
        ResourceManager.GetString(nameof(RequiredEmailErrorMessage), Culture)!;
    public static string EmailMaximumLengthErrorMessage =>
        ResourceManager.GetString(nameof(EmailMaximumLengthErrorMessage), Culture)!;
    public static string InvalidEmailErrorMessage =>
        ResourceManager.GetString(nameof(InvalidEmailErrorMessage), Culture)!;
    public static string InvalidEmailCharactersErrorMessage =>
        ResourceManager.GetString(nameof(InvalidEmailCharactersErrorMessage), Culture)!;
    public static string RequiredPasswordErrorMessage =>
        ResourceManager.GetString(nameof(RequiredPasswordErrorMessage), Culture)!;
    public static string PasswordTooShortErrorMessage =>
        ResourceManager.GetString(nameof(PasswordTooShortErrorMessage), Culture)!;
    public static string PasswordRequiresLowerErrorMessage =>
        ResourceManager.GetString(nameof(PasswordRequiresLowerErrorMessage), Culture)!;
    public static string PasswordRequiresUpperErrorMessage =>
        ResourceManager.GetString(nameof(PasswordRequiresUpperErrorMessage), Culture)!;
    public static string PasswordRequiresDigitErrorMessage =>
        ResourceManager.GetString(nameof(PasswordRequiresDigitErrorMessage), Culture)!;
    public static string PasswordRequiresNonAlphanumericErrorMessage =>
        ResourceManager.GetString(nameof(PasswordRequiresNonAlphanumericErrorMessage), Culture)!;
}
