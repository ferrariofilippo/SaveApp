namespace App.Helpers
{
	public interface INotificationManager
	{
		void Initialize();

		void SendNotification(string title, string message);
	}
}
