using App.Log;
using App.Resx;
using System;
using Xamarin.Forms;

namespace App.Helpers.Notifications
{
    public static class NotificationHelper
    {
        private static readonly INotificationManager _notificationManager = DependencyService.Get<INotificationManager>();

        private static readonly Logger _logger = DependencyService.Get<Logger>();

        public static void SendNotification(string title, string message)
            => _notificationManager.SendNotification(title, message);

        public static void NotifyException(Exception ex)
        {
            var exceptionMessage = $"Thrown by: {ex.TargetSite.Name}\n\rMessage: {ex.Message}";
            SendNotification(AppResource.Error, exceptionMessage);
            _logger.LogWarningAsync(ex);
        }
    }
}
