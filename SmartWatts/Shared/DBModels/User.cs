﻿using System;
using System.Collections.Generic;

namespace SmartWatts.Shared.DBModels
{
    public class User
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public int StravaUserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int FTP { get; set; }
        public string StravaAccessToken { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DateCreated { get; set; }
        public List<Activity> Activities { get; set; }

    }
}
