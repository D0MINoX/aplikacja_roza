namespace MauiApp1.Models
{
    public class AdminUserView
    {
        public int MembershipId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string RosaryName { get; set; }
        public bool IsAuthorized { get; set; }
        public int Role { get; set; }   
    }
}
