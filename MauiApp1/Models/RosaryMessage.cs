using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Models
{
    internal class RosaryMessage
    {
        public int Id { get; set; }
        public int RosaryId { get; set; }
        public string AuthorName { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
