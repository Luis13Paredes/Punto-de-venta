using System.Text.RegularExpressions;

namespace NorthWind.Membership.Frontend.RazorViews.Components;

public static class InputFilters
{
    private static readonly Regex NameAllowedChars =
        new("[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]");

    private static readonly Regex EmailAllowedChars =
        new("[a-zA-Z0-9._@-]");

    private static readonly Regex DigitsAllowedChars =
        new("[0-9]");

    private static readonly Regex AddressAllowedChars =
        new("[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ0-9 .,#°-]");

    private static readonly Regex CompleteDomainAtEnd =
        new(@"@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z]{2,}\.+$");

    public static string FilterName(string? value) =>
        Filter(value, NameAllowedChars);

    public static string FilterEmail(string? value)
    {
        // Nothing is allowed after the top-level domain: strips the
        // trailing dots that arrive from paste or autocomplete
        // (example@mail.com. -> example@mail.com).
        var Filtered = Filter(value, EmailAllowedChars);
        return CompleteDomainAtEnd.IsMatch(Filtered)
            ? CompleteDomainAtEnd.Replace(Filtered, m =>
                m.Value.TrimEnd('.'))
            : Filtered;
    }

    public static string FilterDigits(string? value) =>
        Filter(value, DigitsAllowedChars);

    public static string FilterAddress(string? value) =>
        Filter(value, AddressAllowedChars);

    private static string Filter(string? value, Regex allowedChars)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return new string(value
            .Where(c => allowedChars.IsMatch(c.ToString()))
            .ToArray());
    }
}
