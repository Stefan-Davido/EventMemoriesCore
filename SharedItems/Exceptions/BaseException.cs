using System;
using System.Collections.Generic;
using System.Text;

namespace SharedItems.Exceptions
{
    public class BaseException : Exception
    {

        public int StatusCode { get; set; }
        public string Details { get; set; } = string.Empty;

        public BaseException(string? message) : base(message)
        {
        }
        public BaseException(int statusCode, string message, string? details) : base(message)
        {
            StatusCode = statusCode;
            Details = details ?? string.Empty;
        }
    }
}
