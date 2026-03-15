using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.Models
{
    public class AdminUserView : INotifyPropertyChanged
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserSurname { get; set; }
        public string RosaryName { get; set; }
        public bool IsAuthorized { get; set; }
        public int UserRole { get; set; }
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }
        private List<RoleOption> _availableRoles;
        public List<RoleOption> AvailableRoles
        {
            get => _availableRoles;
            set { _availableRoles = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public string UserRoleName => UserRole switch
        {
            0 => "Admin",
            1 => "Zelator główny",
            2 => "Zelator",
            3 => "Członek róży",
            4 => "Brak",
            _ => $"Rola {UserRole}"
        };
        private RoleOption _selectedRoleObject;
        public RoleOption SelectedRoleObject
        {
            get => _selectedRoleObject;
            set
            {
                if (_selectedRoleObject != value && value != null)
                {
                    _selectedRoleObject = value;
                    UserRole = value.Id; // Automatycznie aktualizuje numer ID dla bazy danych
                    OnPropertyChanged();
                }
            }
        }
    }
}
