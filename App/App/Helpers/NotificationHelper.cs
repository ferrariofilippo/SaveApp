using Xamarin.Forms;

namespace App.Helpers
{
	public static class NotificationHelper
	{
		private static readonly INotificationManager _notificationManager = DependencyService.Get<INotificationManager>();

		public static void SendNotification(string title, string message)
			=> _notificationManager.SendNotification(title, message);
	}
}
