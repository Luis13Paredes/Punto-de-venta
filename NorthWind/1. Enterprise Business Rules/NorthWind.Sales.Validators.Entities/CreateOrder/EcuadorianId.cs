namespace NorthWind.Sales.Validators.Entities.CreateOrder;

/// <summary>
/// Validates Ecuadorian national id numbers (cédula): 13 digits
/// checked with the modulus 11 algorithm.
/// </summary>
public static class EcuadorianId
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 13)
            return false;

        if (!value.All(char.IsDigit))
            return false;

        int Sum = 0;
        for (int Index = 0; Index < 12; Index++)
        {
            int Digit = value[Index] - '0';
            int Weight = (Index % 2 == 0) ? 1 : 2;
            int Product = Digit * Weight;

            Sum += Product > 9 ? Product - 9 : Product;
        }

        int VerificationDigit = (10 - (Sum % 10)) % 10;

        return VerificationDigit == value[12] - '0';
    }
}
