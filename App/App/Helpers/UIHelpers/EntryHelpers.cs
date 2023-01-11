using System.Globalization;
using Xamarin.Forms;

namespace App.Helpers.UIHelpers
{
    public static class EntryHelpers
    {
        public static void AdjustCurrencyEntryText(Entry entry, TextChangedEventArgs e)
        {
            if (entry is null || e is null || e.NewTextValue.Length == 0)
                return;
            var lastIndex = e.NewTextValue.Length - 1;
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," &&
                e.NewTextValue[lastIndex] == '.')
            {
                entry.Text = e.OldTextValue + ",";
            }
            else if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." &&
                e.NewTextValue[lastIndex] == ',')
            {
                entry.Text = e.OldTextValue + ".";
            }
        }
    }
}
