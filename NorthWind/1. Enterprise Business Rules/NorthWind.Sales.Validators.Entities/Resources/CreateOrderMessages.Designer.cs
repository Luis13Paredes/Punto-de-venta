#nullable enable
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NorthWind.Sales.Validators.Entities.Resources;

public static class CreateOrderMessages
{
    static readonly ResourceManager ResourceManager = new(
        "NorthWind.Sales.Validators.Entities.Resources.CreateOrderMessages",
        typeof(CreateOrderMessages).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string ProductIdGreaterThanZero =>
        ResourceManager.GetString(nameof(ProductIdGreaterThanZero), Culture)!;
    public static string QuantityGreaterThanZero =>
        ResourceManager.GetString(nameof(QuantityGreaterThanZero), Culture)!;
    public static string UnitPriceGreaterThanZero =>
        ResourceManager.GetString(nameof(UnitPriceGreaterThanZero), Culture)!;
    public static string CustomerIdRequired =>
        ResourceManager.GetString(nameof(CustomerIdRequired), Culture)!;
    public static string CustomerIdRequiredLength =>
        ResourceManager.GetString(nameof(CustomerIdRequiredLength), Culture)!;
    public static string CustomerIdInvalidFormat =>
        ResourceManager.GetString(nameof(CustomerIdInvalidFormat), Culture)!;
    public static string ShipCityOnlyLetters =>
        ResourceManager.GetString(nameof(ShipCityOnlyLetters), Culture)!;
    public static string ShipCountryOnlyLetters =>
        ResourceManager.GetString(nameof(ShipCountryOnlyLetters), Culture)!;
    public static string ShipPostalCodeRequired =>
        ResourceManager.GetString(nameof(ShipPostalCodeRequired), Culture)!;
    public static string ShipPostalCodeOnlyDigits =>
        ResourceManager.GetString(nameof(ShipPostalCodeOnlyDigits), Culture)!;
    public static string ShipAddressRequired =>
        ResourceManager.GetString(nameof(ShipAddressRequired), Culture)!;
    public static string ShipAddressMaximumLength =>
        ResourceManager.GetString(nameof(ShipAddressMaximumLength), Culture)!;
    public static string ShipCityRequired =>
        ResourceManager.GetString(nameof(ShipCityRequired), Culture)!;
    public static string ShipCityMinimumLength =>
        ResourceManager.GetString(nameof(ShipCityMinimumLength), Culture)!;
    public static string ShipCityMaximumLength =>
        ResourceManager.GetString(nameof(ShipCityMaximumLength), Culture)!;
    public static string ShipCountryRequired =>
        ResourceManager.GetString(nameof(ShipCountryRequired), Culture)!;
    public static string ShipCountryMinimumLength =>
        ResourceManager.GetString(nameof(ShipCountryMinimumLength), Culture)!;
    public static string ShipCountryMaximumLength =>
        ResourceManager.GetString(nameof(ShipCountryMaximumLength), Culture)!;
    public static string ShipPostalCodeMaximumLength =>
        ResourceManager.GetString(nameof(ShipPostalCodeMaximumLength), Culture)!;
    public static string OrderDetailsRequired =>
        ResourceManager.GetString(nameof(OrderDetailsRequired), Culture)!;
    public static string OrderDetailsNotEmpty =>
        ResourceManager.GetString(nameof(OrderDetailsNotEmpty), Culture)!;
}
