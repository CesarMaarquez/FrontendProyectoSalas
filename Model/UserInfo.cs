namespace AppSalas.Model
{
    public class UserInfoModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dni { get; set; }
        public int age { get; set; }
        public string gender { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public int AniosExperiencia { get; set; }
        public string Especialidad { get; set; }
        public string Institution { get; set; }
        public string Degree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Specialties { get; set; }
        public List<string> Struggles { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string UserType { get; set; }
        public int userId { get; set; }
    }
}
