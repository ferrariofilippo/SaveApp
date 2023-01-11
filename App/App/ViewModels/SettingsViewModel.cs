using App.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.Linq;

namespace App.ViewModels
{
	public class SettingsViewModel : ObservableObject
	{
		private const string _chevronUpPath = "chevronUp.png";

		private const string _chevronDownPath = "chevronDown.png";

		private static CultureInfo culutre => CultureInfo.CurrentCulture;

		public readonly string[] Themes = Enum.GetValues(typeof(Theme))
			.Cast<Theme>()
			.Select(x => App.ResourceManager.GetString(x.ToString(), culutre))
			.ToArray();

		public readonly string[] Currencies = Enum.GetValues(typeof(Currencies))
			.Cast<Currencies>()
			.Select(x => x.ToString())
			.ToArray();

		private bool _isDataSectionVisible;
		public bool IsDataSectionVisible
		{
			get => _isDataSectionVisible;
			set
			{
				if (SetProperty(ref _isDataSectionVisible, value))
					OnPropertyChanged(nameof(ToggleIconSource));
			}
		}

		public string ToggleIconSource => IsDataSectionVisible
			? _chevronUpPath
			: _chevronDownPath;
	}
}
