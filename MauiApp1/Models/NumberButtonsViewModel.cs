using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp1.Models
{
    internal class NumberButtonsViewModel
    {
        public ObservableCollection<int> Numbers { get; } = new(Enumerable.Range(0, 32));

        public ICommand NumberTappedCommand { get; }

        public NumberButtonsViewModel()
        {
            NumberTappedCommand = new Command<int>(OnNumberTapped);
        }

        private void OnNumberTapped(int number)
        {
            // tutaj Twoja logika, np. wyświetlenie alertu
            System.Diagnostics.Debug.WriteLine($"Kliknięto przycisk numer: {number}");
        }
    }
}
