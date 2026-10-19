#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Sales.Frontend.Views.Resources;

public static class CommonMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Sales.Frontend.Views.Resources.CommonMessages",
        typeof(CommonMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string OK =>
        ResourceManager.GetString(nameof(OK), Culture)!;
    public static string NavBarBrandTitle =>
        ResourceManager.GetString(nameof(NavBarBrandTitle), Culture)!;
    public static string HomeMenuItemTitle =>
        ResourceManager.GetString(nameof(HomeMenuItemTitle), Culture)!;
    public static string AboutTitle =>
        ResourceManager.GetString(nameof(AboutTitle), Culture)!;
    public static string WelcomeHeading =>
        ResourceManager.GetString(nameof(WelcomeHeading), Culture)!;
    public static string WelcomeSubtitle =>
        ResourceManager.GetString(nameof(WelcomeSubtitle), Culture)!;
    public static string WelcomeNewOrderText =>
        ResourceManager.GetString(nameof(WelcomeNewOrderText), Culture)!;
    public static string WelcomeInvoicesText =>
        ResourceManager.GetString(nameof(WelcomeInvoicesText), Culture)!;
    public static string LoginRequiredHint =>
        ResourceManager.GetString(nameof(LoginRequiredHint), Culture)!;
    public static string ExploreOptionsText =>
        ResourceManager.GetString(nameof(ExploreOptionsText), Culture)!;
    public static string NotFoundTitle =>
        ResourceManager.GetString(nameof(NotFoundTitle), Culture)!;
    public static string NotFoundText =>
        ResourceManager.GetString(nameof(NotFoundText), Culture)!;
}
