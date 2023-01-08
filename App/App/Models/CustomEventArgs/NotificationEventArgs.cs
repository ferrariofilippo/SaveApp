using System;

namespace App.Models.CustomEventArgs
{
	public class NotificationEventArgs : EventArgs
	{
		public string Title { get; set; }
		public string Message { get; set; }
	}
}
