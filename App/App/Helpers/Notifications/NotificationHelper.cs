﻿using App.Helpers.LogHelper;
using App.Resx;
using System;
using Xamarin.Forms;

namespace App.Helpers.Notifications
{
    public static class NotificationHelper
    {
        private static readonly INotificationManager _notificationManager = DependencyService.Get<INotificationManager>();

        private static readonly ILogger _logger = DependencyService.Get<ILogger>();

        public static void SendNotification(string title, string message)
            => _notificationManager.SendNotification(title, message);

        public static void NotifyException(Exception ex)
        {
            if (ex is null)
                return;
            var exceptionMessage = $"Thrown by: {ex.TargetSite.Name}\n\rMessage: {ex.Message}";
            SendNotification(AppResource.Error, exceptionMessage);
            _logger.LogWarningAsync(ex);
        }
    }
}
