using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Misc
{
    internal class StreamOrganizationDataset
    {
      public  Camera Camera { get; set; }
        public ExternalProgramExecutor Process { get; set; }
        public ushort Port{ get; set; }
        public string Path{ get; set; }
    }
}
