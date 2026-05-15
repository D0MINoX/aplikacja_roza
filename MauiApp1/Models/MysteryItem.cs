using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.Models
{
    public class MysteryItem
    {
        public string ShortLabel { get; set; }
        public string FullDescription { get; set; }

        public MysteryItem(string shortLabel, string fullDescription)
        {
            ShortLabel = shortLabel;
            FullDescription = fullDescription;
        }
    }
}
