namespace ConSurvBackend.Core.Model
{
    public class UserInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }

        public UserInformation(string id, string name, bool isAdmin)
        {
            this.Id = id;
            this.Name = name;
            this.IsAdmin = this.IsAdmin;
        }
    }
}
