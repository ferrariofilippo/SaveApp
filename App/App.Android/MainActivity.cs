using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

namespace App.Droid
{
    [Activity(Label = "App", Icon = "@mipmap/icon", Theme = "@style/lightAppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
			Instance = null;
			var theme = ThemeManager.GetCurrentTheme();
			switch (theme)
			{
				case Models.Enums.Theme.Light:
					SetTheme(Resource.Style.lightAppTheme);
					break;
				case Models.Enums.Theme.Nord:
					SetTheme(Resource.Style.nordAppTheme);
					break;
				case Models.Enums.Theme.Ice:
					SetTheme(Resource.Style.iceAppTheme);
					break;
				case Models.Enums.Theme.Teal:
					SetTheme(Resource.Style.tealAppTheme);
					break;
			}

			base.OnCreate(savedInstanceState);
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
			LoadApplication(new App());

			Instance = this;
		}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}