namespace App.Helpers.Notifications
{
    /// <summary>
    /// An object that grants the ability to send notifications
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Initialize the object setting-up the environment to send notifications
        /// </summary>
        void Initialize();

		/// <summary>
		/// Asynchronously notifies the user
		/// </summary>
		/// <param name="title">Notification title</param>
		/// <param name="message">Notification message</param>
		void SendNotification(string title, string message);
    }
}
