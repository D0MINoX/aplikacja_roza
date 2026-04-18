using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MauiApp1.Models
{
    public class ErrorReport : INotifyPropertyChanged
    {
        private string? _userPhone;
        private string _errorMessage = string.Empty;
        private string _status = "Nowe";
        private bool _isEditing;

        public int Id { get; set; }

        // Numer telefonu użytkownika (opcjonalne)
        public string? UserPhone
        {
            get => _userPhone;
            set
            {
                if (_userPhone != value)
                {
                    _userPhone = value;
                    OnPropertyChanged(nameof(UserPhone));
                }
            }
        }

        // Wiadomość błędu/zgłoszenia
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        // Status zgłoszenia błędu (np. Nowe, Odebrane, Zamknięte)
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Flaga trybu edycji danego zgłoszenia
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged(nameof(IsEditing));
                }
            }
        }

        public List<string> AvailableStatuses { get; set; } = new List<string>
{
    "Nowe",
    "Odebrane",
    "Zamknięte"
};

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}