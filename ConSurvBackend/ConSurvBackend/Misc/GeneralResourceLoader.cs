namespace ConSurvBackend.Core.Misc
{
    /// <summary>
    /// Application-specific resource loader that resolves embedded resources from the
    /// <c>ConSurvBackend.Core.Resources</c> namespace within the entry assembly.
    /// </summary>
    public class GeneralResourceLoader : GRYLibrary.Core.APIServer.Services.Res.GeneralResourceLoader
    {
        /// <inheritdoc />
        public GeneralResourceLoader() : base("ConSurvBackend.Core.Resources", typeof(Program).Assembly) { }
    }
}
