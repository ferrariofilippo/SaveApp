using System;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace App.Helpers.LogHelper
{
	public class Logger : ILogger
	{
		private const int BUFFER_SIZE = 2048;
		private const int STRING_BUILDER_CAPACITY = 256;

		private readonly string _logFilePath;

		public Logger()
		{
			_logFilePath = Path.Combine(FileSystem.CacheDirectory, Constants.LOG_PATH);
		}

		public async void LogWarningAsync(Exception exception)
		{
			if (exception is null)
				return;
			using (var fs = TryGetLogStream())
			{
				if (fs is null)
					return;

				var formattedException = FormatException(exception);
				var offset = 0;
				var bytes = Encoding.UTF8.GetBytes(formattedException);
				while (offset < bytes.Length)
				{
					await fs.WriteAsync(bytes, offset, BUFFER_SIZE);
					offset += BUFFER_SIZE;
				}
			}
		}

		private FileStream TryGetLogStream()
		{
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BUFFER_SIZE);
			}
			catch (IOException) { }
			return fileStream;
		}

		private string FormatException(Exception exception)
		{
			var builder = new StringBuilder(STRING_BUILDER_CAPACITY);

			builder.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
			builder.Append($"|WARN|{exception.TargetSite.Name,30}|");
			builder.AppendLine(exception.Message);
			builder.AppendLine(exception.StackTrace);

			return builder.ToString();
		}
	}
}
