using System;
namespace GetTradeHistoryData
{
    public class EmptyLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            return;
        }
    }
}
