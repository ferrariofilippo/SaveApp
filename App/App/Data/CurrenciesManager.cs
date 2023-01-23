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
	public class CurrenciesManager : ICurrenciesManager, IDisposable
	{
		private readonly ISettingsManager _settings = DependencyService.Get<ISettingsManager>();

		private readonly string _cachePath;

		private readonly HttpClient _client = new HttpClient()
		{
			BaseAddress = new Uri(Constants.BASE_CURRENCIES_URL)
		};

		private readonly decimal[] _rates = new decimal[8];

		public DateTime LastUpdate { get; private set; }

		public CurrenciesManager()
		{
			for (int i = 0; i < _rates.Length; i++)
				_rates[i] = 1.0m;

			_cachePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Constants.CURRENCIES_CACHE_PATH);

			if (File.Exists(_cachePath))
				LoadCached();
			else
				LoadLatest();
		}

		public decimal ConvertCurrencyToDefault(decimal value, Currencies from)
			=> value * _rates[_settings.Settings.BaseCurrency] / _rates[(byte)from];

		public async Task UpdateAllToCurrent(Currencies previous)
		{
			var stats = DependencyService.Get<StatisticsManager>();
			var db = DependencyService.Get<IAppDatabase>();

			var changeRatio = _rates[_settings.Settings.BaseCurrency] / _rates[(int)previous];

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
						_rates[i] = parsed.Rates[i];
			}
			else
				LoadLatest();
		}

		private async Task SaveCached()
		{
			var cached = new CurrenciesCache
			{
				LastUpdated = LastUpdate,
				Rates = _rates
			};

			using (var writer = new StreamWriter(_cachePath, false))
				await writer.WriteAsync(JsonSerializer.Serialize(cached));
		}

		private async void LoadLatest()
		{
			var data = await Fetch("latest");
			if (data is null)
				return;

			_rates[1] = data["USD"];
			_rates[2] = data["AUD"];
			_rates[3] = data["CAD"];
			_rates[4] = data["GBP"];
			_rates[5] = data["CHF"];
			_rates[6] = data["JPY"];
			_rates[7] = data["CNY"];

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

		public void Dispose()
		{
			_client.Dispose();
			GC.SuppressFinalize(this);
		}

		private sealed class CurrenciesCache
		{
			public DateTime LastUpdated { get; set; }

			public decimal[] Rates { get; set; }
		}
	}
}
