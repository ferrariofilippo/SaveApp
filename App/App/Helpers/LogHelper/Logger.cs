using System;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace App.Helpers.LogHelper
{
    public class Logger
    {
        private const int _bufferSize = 4096;

        private readonly string _logFilePath;

        public Logger()
        {
            _logFilePath = Path.Combine(FileSystem.CacheDirectory, Constants.LogPath);
        }

        public async void LogWarningAsync(Exception exception)
        {
            var fs = TryGetLogStream();
            if (fs is null)
                return;

            var formattedException = FormatException(exception);
            if (formattedException is null)
                return;

            var offset = 0;
            var bytes = Encoding.UTF8.GetBytes(formattedException);
            while (offset < bytes.Length)
            {
                await fs.WriteAsync(bytes, offset, _bufferSize);
                offset += _bufferSize;
            }
        }

        private FileStream TryGetLogStream()
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            }
            catch (Exception) { }
            return fileStream;
        }

        private string FormatException(Exception exception)
        {
            var builder = new StringBuilder(300);

            builder.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff"));
            builder.Append($"|WARN|{exception.TargetSite.Name,30}|");
            builder.AppendLine(exception.Message);
            builder.AppendLine(exception.StackTrace);

            return builder.ToString();
        }
    }
}
