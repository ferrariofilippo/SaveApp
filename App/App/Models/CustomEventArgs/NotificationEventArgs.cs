using System;

namespace App.Models.CustomEventArgs
{
	public sealed class NotificationEventArgs : EventArgs
	{
		public string Title { get; set; }
		public string Message { get; set; }
	}
}
