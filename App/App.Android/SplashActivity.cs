﻿using Android.App;
using Android.Content;
using Android.OS;

namespace App.Droid
{
	[Activity(Theme = "@style/AppCompatSplashScreen", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		protected override void OnResume()
		{
			base.OnResume();
			StartActivity(new Intent(Application.Context, typeof(MainActivity)));
		}
	}
}