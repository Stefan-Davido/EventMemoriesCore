using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedItems.Exceptions
{
    public class UnauthorizeException : BaseException
    {
        public UnauthorizeException(string message) : base(StatusCodes.Status401Unauthorized, message, null)
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
