using App.Data;
using App.Models.Enums;
using App.Helpers;
using Xamarin.Forms;

namespace App.Extensions
{
	public static class DecimalExtension
	{
		private static readonly SettingsManager _settings = DependencyService.Get<SettingsManager>();

		public static string ToCurrencyString(this decimal value) 
			=> $"{(value > 0 ? '+' : '\0')} {value:0.00}{CurrencyHelper.GetCurrencySymbol((Currencies)_settings.Settings.BaseCurrency)}";
	}
}
