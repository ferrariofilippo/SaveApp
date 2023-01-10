namespace App.Helpers.Notifications
{
    public interface INotificationManager
    {
        void Initialize();

        void SendNotification(string title, string message);
    }
}
