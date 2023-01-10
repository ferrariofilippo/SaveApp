using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using App.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(App.Droid.AndroidNotificationManager))]
namespace App.Droid
{
	public class AndroidNotificationManager : INotificationManager
	{
		private const string _channelId = "default";
		private const string _channelName = "Default";
		private const string _channelDescription = "Default channel for notifications";

		public const string TitleKey = "title";
		public const string MessageKey = "message";

		private bool _channelInitialized = false;
		private int _messageId = 0;
		private int _pendingIntentId = 0;

		private NotificationManager _manager;

		public static AndroidNotificationManager Instance { get; private set; }

		public AndroidNotificationManager() => Initialize();

		public void Initialize()
		{
			if (!(Instance is null))
				return;
			CreateNotificationChannel();
			Instance = this;
		}

		public void SendNotification(string title, string message)
		{
			if (!_channelInitialized)
				CreateNotificationChannel();

			Show(title, message);
		}

		public void Show(string title, string message)
		{
			var intent = new Intent(Android.App.Application.Context, typeof(MainActivity));
			intent.PutExtra(TitleKey, title);
			intent.PutExtra(MessageKey, message);

			var intentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
				? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable
				: PendingIntentFlags.UpdateCurrent;

			var pending = PendingIntent.GetActivity(
				Android.App.Application.Context,
				_pendingIntentId++,
				intent,
				intentFlags);

			var builder = new NotificationCompat.Builder(Android.App.Application.Context, _channelId)
				.SetContentIntent(pending)
				.SetContentTitle(title)
				.SetContentText(message)
				.SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Mipmap.icon_round))
				.SetSmallIcon(Resource.Mipmap.icon_round)
				.SetDefaults((int)NotificationDefaults.All);

			var notification = builder.Build();
			_manager.Notify(_messageId++, notification);
		}

		private void CreateNotificationChannel()
		{
			_manager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService);

			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				var channelNameJava = new Java.Lang.String(_channelName);
				var channel = new NotificationChannel(_channelId, channelNameJava, NotificationImportance.Default)
				{
					Description = _channelDescription
				};
				_manager.CreateNotificationChannel(channel);
			}

			_channelInitialized = true;
		}
	}
}