using App.Models.Enums;
using System;
using System.Linq;

namespace App.ViewModels
{
	public class SettingsViewModel
	{
		public readonly string[] Themes = Enum.GetValues(typeof(Theme))
			.Cast<Theme>()
			.Select(x => App.ResourceManager.GetString(x.ToString()))
			.ToArray();

		public readonly string[] Currencies = Enum.GetValues(typeof(Currencies))
			.Cast<Currencies>()
			.Select(x => x.ToString())
			.ToArray();
	}
}
