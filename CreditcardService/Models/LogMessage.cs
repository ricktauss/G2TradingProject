﻿namespace CreditcardService.Models
{
    public class LogMessage
    {
        /// <summary>
        /// Timestamp in UTC of the message occurrence
        /// </summary>
        public string TimestampUtc { get; set; }

        /// <summary>
        /// Message as text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Supported log levels:
        /// 1: Information
        /// 2: Warning
        /// 3: Error
        /// </summary>
        public int LogLevel { get; set; }
    }
}
