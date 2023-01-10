using App.Helpers.Notifications;
using App.Models.Enums;
using App.Resx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.Data
{
    public class CurrenciesManager
	{
		private readonly SettingsManager _settings = DependencyService.Get<SettingsManager>();

		private readonly string _cachePath;

		private readonly HttpClient _client = new HttpClient()
		{
			BaseAddress = new Uri(Constants.BaseCurrenciesUrl)
		};

		public readonly decimal[] Rates = new decimal[8];

		public DateTime LastUpdate { get; private set; }

		public CurrenciesManager()
		{
			_cachePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Constants.CurrenciesCachePath);

			for (int i = 0; i < Rates.Length; i++)
				Rates[i] = 1.0m;

			if (File.Exists(_cachePath))
				LoadCached();
			else
				LoadLatest();
		}

		public decimal ConvertCurrencyToDefault(decimal value, Currencies from) 
			=> value * Rates[_settings.Settings.BaseCurrency] / Rates[(byte)from];

		public async Task UpdateAllToCurrent(Currencies previous)
		{
			var stats = DependencyService.Get<StatisticsHolder>();
			var db = DependencyService.Get<AppDatabase>();

			var factor = Rates[_settings.Settings.BaseCurrency] / Rates[(int)previous];

			await Task.WhenAll(
				stats.UpdateAllToNewCurrency(factor),
				db.UpdateDbToNewCurrency(factor));

			NotificationHelper.SendNotification(
				AppResource.DatabaseCurrencyUpdateEnded, 
				AppResource.DatabseCurrencyUpdateEndedMessage);
		}

		private void LoadCached()
		{
			var json = File.ReadAllText(_cachePath);
			if (!string.IsNullOrWhiteSpace(json))
			{
				var parsed = JsonSerializer.Deserialize<CurrenciesCache>(json);

				for (int i = 0; i < parsed.Rates.Length; i++)
					Rates[i] = parsed.Rates[i];

				if (parsed.LastUpdated < DateTime.Today)
					LoadLatest();
			}
			else
			{
				LoadLatest();
			}
		}

		private async Task SaveCached()
		{
			var toSave = new CurrenciesCache
			{
				LastUpdated = LastUpdate,
				Rates = Rates
			};

			using (var writer = new StreamWriter(_cachePath, false))
				await writer.WriteAsync(JsonSerializer.Serialize(toSave));
		}

		private async void LoadLatest()
		{
			var data = await Fetch("latest");
			if (data is null)
				return;

			Rates[1] = data["USD"];
			Rates[2] = data["AUD"];
			Rates[3] = data["CAD"];
			Rates[4] = data["GBP"];
			Rates[5] = data["CHF"];
			Rates[6] = data["JPY"];
			Rates[7] = data["CNY"];

			await SaveCached();
		}

		private async Task<Dictionary<string, decimal>> Fetch(string query)
		{
			HttpResponseMessage response = await _client.GetAsync(query);
			if (!response.IsSuccessStatusCode)
				return null;
			var parsed = JsonSerializer.Deserialize<JsonDocument>(
				await response.Content.ReadAsStringAsync())
				.RootElement
				.GetProperty("rates");

			LastUpdate = DateTime.Today;

			return JsonSerializer.Deserialize<Dictionary<string, decimal>>(parsed.GetRawText());
		}

		private class CurrenciesCache
		{
			public DateTime LastUpdated { get; set; }

			public decimal[] Rates { get; set; }
		}
	}
}
