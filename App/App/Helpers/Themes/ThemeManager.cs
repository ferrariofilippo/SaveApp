using App.Data;
using App.Models.Enums;
using App.Themes;
using Xamarin.Forms;

namespace App.Helpers.Themes
{
    public class ThemeManager
    {
        public static SettingsManager Settings = null;

        public static async void ChangeTheme(Theme theme)
        {
            if (Settings is null || Device.RuntimePlatform != Device.Android)
                return;

            var mergedDictionaires = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaires is null)
                return;

            var themeByte = (byte)theme;
            Settings.Settings.Theme = themeByte;
            await Settings.SaveSettings();

            mergedDictionaires.Clear();

            switch (theme)
            {
                case Theme.Light:
                    mergedDictionaires.Add(new LightTheme());
                    Application.Current.UserAppTheme = OSAppTheme.Light;
                    break;
                case Theme.Nord:
                    mergedDictionaires.Add(new NordTheme());
                    Application.Current.UserAppTheme = OSAppTheme.Dark;
                    break;
                case Theme.Ice:
                    mergedDictionaires.Add(new IceTheme());
                    Application.Current.UserAppTheme = OSAppTheme.Light;
                    break;
                case Theme.Teal:
                    mergedDictionaires.Add(new TealTheme());
                    Application.Current.UserAppTheme = OSAppTheme.Dark;
                    break;
            }

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
