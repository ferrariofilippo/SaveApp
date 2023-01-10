using App.Helpers.Themes;
using App.Models.Enums;
using Xamarin.Forms;

[assembly: Dependency(typeof(App.Droid.AndroidThemeManager))]
namespace App.Droid
{
    public class AndroidThemeManager : INativeThemeManager
	{
		public void OnThemeChanged(Theme theme)
		{
			var activity = MainActivity.Instance;
			if (activity is null)
				return;
			var intent = MainActivity.Instance.Intent;
			activity.Finish();
			activity.StartActivity(intent);
		}
	}
}