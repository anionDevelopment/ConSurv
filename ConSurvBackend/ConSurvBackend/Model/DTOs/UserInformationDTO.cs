namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object that exposes basic identity and role information about
    /// a logged-in user to the client.
    /// </summary>
    public class UserInformationDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }

        /// <summary>
        /// Initializes a new <see cref="UserInformationDTO"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="name">The display name of the user.</param>
        /// <param name="isAdmin">Whether the user holds administrator privileges.</param>
        /// <param name="isModerator">Whether the user holds moderator privileges.</param>
        public UserInformationDTO(string id, string name, bool isAdmin, bool isModerator)
        {
            this.Id = id;
            this.Name = name;
            this.IsAdmin = isAdmin;
            this.IsModerator = isModerator;
        }
    }
}
