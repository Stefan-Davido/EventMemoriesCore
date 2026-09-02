using System;
using System.Collections.Generic;
using System.Text;

namespace EventMemoriesServices.DTOs
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserDto User { get; set; }
        public int ExpiresIn { get; set; }
    }
}