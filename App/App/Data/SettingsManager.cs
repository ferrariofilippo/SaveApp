using App.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Data
{
	public sealed class SettingsManager : ISettingsManager
	{
		private readonly string _settingsPath;

		public Settings Settings { get; private set; } = new Settings();

		public SettingsManager()
		{
			_settingsPath = Path.Combine(
				Environment.GetFolderPath(
					Environment.SpecialFolder.LocalApplicationData),
				Constants.SETTINGS_PATH);

			if (File.Exists(_settingsPath))
			{
				var json = File.ReadAllText(_settingsPath);
				if (!string.IsNullOrWhiteSpace(json))
					Settings = JsonSerializer.Deserialize<Settings>(json);
			}
		}

		public async Task SaveSettings()
		{
			using (var writer = new StreamWriter(_settingsPath, false))
				await writer.WriteAsync(JsonSerializer.Serialize(Settings));
		}
	}
}
