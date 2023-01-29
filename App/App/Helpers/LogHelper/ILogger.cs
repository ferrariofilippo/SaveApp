using System;

namespace App.Helpers.LogHelper
{
    /// <summary>
    /// An object that provide the ability to write logs
    /// </summary>
    public interface ILogger
    {
		/// <summary>
		/// Asynchronously logs an exception as Warning
		/// </summary>
		/// <param name="exception">Exception to be logged</param>
		void LogWarningAsync(Exception exception);
    }
}
