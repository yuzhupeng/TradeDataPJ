using System;
namespace GetTradeHistoryData
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            string dateTime = DateTime.UtcNow.ToString("s");
            Console.WriteLine($"{dateTime} | {level} | {message}");
        }
    }
}
