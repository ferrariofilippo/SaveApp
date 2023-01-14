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
			for (int i = 0; i < Rates.Length; i++)
				Rates[i] = 1.0m;

			_cachePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Constants.CurrenciesCachePath);

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

			var changeRatio = Rates[_settings.Settings.BaseCurrency] / Rates[(int)previous];

			await Task.WhenAll(
				stats.UpdateAllToNewCurrency(changeRatio),
				db.UpdateDbToNewCurrency(changeRatio));

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

				if (parsed.LastUpdated < DateTime.Today)
					LoadLatest();
				else
					for (int i = 0; i < parsed.Rates.Length; i++)
						Rates[i] = parsed.Rates[i];
			}
			else
				LoadLatest();
		}

		private async Task SaveCached()
		{
			var cached = new CurrenciesCache
			{
				LastUpdated = LastUpdate,
				Rates = Rates
			};

			using (var writer = new StreamWriter(_cachePath, false))
				await writer.WriteAsync(JsonSerializer.Serialize(cached));
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
			try
			{
				HttpResponseMessage response = await _client.GetAsync(query);
				if (!response.IsSuccessStatusCode)
					return null;
				var ratesString = JsonSerializer.Deserialize<JsonDocument>(
					await response.Content.ReadAsStringAsync())
					.RootElement
					.GetProperty("rates")
					.GetRawText();

				LastUpdate = DateTime.Today;

				return JsonSerializer.Deserialize<Dictionary<string, decimal>>(ratesString);
			}
			catch (Exception) { }
			return null;
		}

		private class CurrenciesCache
		{
			public DateTime LastUpdated { get; set; }

			public decimal[] Rates { get; set; }
		}
	}
}
