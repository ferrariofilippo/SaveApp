using App.Data;
using App.Helpers;
using App.Models.Enums;
using App.Themes;
using Xamarin.Forms;

namespace App
{
	public class ThemeManager
	{
		public static SettingsManager Settings = null;

		public static async void ChangeTheme(Theme theme) 
		{
			var themeByte = (byte)theme;
			if (Settings is null)
				return;

			var mergedDictionaires = Application.Current.Resources.MergedDictionaries;
			if (mergedDictionaires is null)
				return;
			
			Settings.Settings.Theme = themeByte;
			await Settings.SaveSettings();

			mergedDictionaires.Clear();

			switch (theme)
			{
				case Theme.Light:
					mergedDictionaires.Add(new LightTheme());
					App.Current.UserAppTheme = OSAppTheme.Light;
					break;
				case Theme.Nord:
					mergedDictionaires.Add(new NordTheme());
					App.Current.UserAppTheme = OSAppTheme.Dark;
					break;
				case Theme.Ice:
					mergedDictionaires.Add(new IceTheme());
					App.Current.UserAppTheme = OSAppTheme.Light;
					break;
				case Theme.Teal:
					mergedDictionaires.Add(new TealTheme());
					App.Current.UserAppTheme = OSAppTheme.Dark;
					break;
			}

			if (Device.RuntimePlatform == Device.Android)
				DependencyService.Get<INativeThemeManager>().OnThemeChanged(theme);
		}

		public static void LoadTheme() 
		{
			if (Settings is null)
				return;
			ChangeTheme((Theme)Settings.Settings.Theme);
		}

		public static Theme GetCurrentTheme()
		{
			var settings = new SettingsManager();
			return settings is null 
				? 0
				: (Theme)settings.Settings.Theme;
		}
	}
}
