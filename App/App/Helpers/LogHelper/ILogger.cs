using System;

namespace App.Helpers.LogHelper
{
    public interface ILogger
    {
        void LogWarningAsync(Exception exception);
    }
}
