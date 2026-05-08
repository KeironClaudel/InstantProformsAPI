namespace InstantProforms.Domain.Enums;

/// <summary>
/// Provides formatting helpers for <see cref="ProformCurrency"/>.
/// </summary>
public static class ProformCurrencyExtensions
{
    /// <summary>
    /// Gets the display symbol for a currency.
    /// </summary>
    /// <param name="currency">The currency value.</param>
    /// <returns>The currency symbol.</returns>
    public static string GetSymbol(this ProformCurrency currency)
    {
        return currency switch
        {
            ProformCurrency.Colones => "₡",
            ProformCurrency.Dollars => "$",
            _ => "₡"
        };
    }
}
