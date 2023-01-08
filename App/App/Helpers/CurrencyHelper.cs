using App.Models.Enums;

namespace App.Helpers
{
    class CurrencyHelper
    {
        public static string GetCurrencySymbol(Currencies currency)
        {
            switch (currency)
            {
                case Currencies.EUR:
                    return "€";
                case Currencies.GBP:
                    return "£";
                case Currencies.CHF:
                    return "Fr";
                case Currencies.JPY:
                case Currencies.CNY:
                    return "¥";
                default:
                    return "$";
            }
        }
    }
}
