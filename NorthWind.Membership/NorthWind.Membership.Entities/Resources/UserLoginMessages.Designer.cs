#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Membership.Entities.Resources;

public static class UserLoginMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Membership.Entities.Resources.UserLoginMessages",
        typeof(UserLoginMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

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
}
