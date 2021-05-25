using UnityEngine;
using Yggdrasil.Logging;

namespace Assets.Scripts
{
    public class UnityConsoleLogTarget : LoggerTarget
    {
        public override string GetFormat(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info: return "\u001B^c13;[{0}][{1}][{2}]\u001B^r; - {3}"; // White
                case LogLevel.Warning: return "\u001B^c14;[{0}][{1}][{2}]\u001B^r; - {3}"; // Yellow
                case LogLevel.Error: return "\u001B^c12;[{0}][{1}][{2}]\u001B^r; - {3}"; // Red
                case LogLevel.Debug: return "\u001B^c8;[{0}][{1}][{2}]\u001B^r; - {3}"; // Dark Gray
                case LogLevel.Status: return "\u001B^c10;[{0}][{1}][{2}]\u001B^r; - {3}"; // Green
            }

            return "[{0}] - {1}";
        }

        public override void Write(LogLevel level, string message, string messageRaw, string messageClean)
        {
            if (level == LogLevel.Info || level == LogLevel.Debug)
                Debug.Log(messageClean);
            if (level == LogLevel.Error)
                Debug.LogError(messageClean);
            if (level == LogLevel.Warning)
                Debug.LogWarning(messageClean);
        }
    }
}
