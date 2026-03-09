using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
    }
}
