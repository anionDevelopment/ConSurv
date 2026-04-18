namespace ConSurvBackend.Core.Services
{
    public interface IExampleDataCreator
    {
        /// <summary>
        /// Populates the system with predefined example users and other seed data for development and demonstration purposes.
        /// </summary>
        public void AddExampleData();
    }
}
