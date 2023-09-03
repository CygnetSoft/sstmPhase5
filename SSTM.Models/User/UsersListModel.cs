namespace SSTM.Models.User
{
    public class UsersListModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TrainingCenter { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public long? Trainer_AirLine_id { get; set; }
        public bool isActive { get; set; }
        
    }
}