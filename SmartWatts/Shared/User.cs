﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWatts.Shared
{
    public class User
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public string Password { get; set; }
        public string StravaAccessToken { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
