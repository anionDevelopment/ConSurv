namespace ConSurvBackend.Core.Model.DTOs
{
    public class UserInformationDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }

        public UserInformationDTO(string id, string name, bool isAdmin, bool isModerator)
        {
            this.Id = id;
            this.Name = name;
            this.IsAdmin = isAdmin;
            this.IsModerator = isModerator;
        }
    }
}
