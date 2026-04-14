using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MauiApp1.Models
{
    public class ExternalNumber:INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int RosaryId { get; set; }
        public DateTime CreatedAt { get; set; }
        private bool isEditing;
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                if (isEditing != value)
                {
                    isEditing = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
