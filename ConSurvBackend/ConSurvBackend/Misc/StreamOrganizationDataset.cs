using ConSurvBackend.Core.Model.Base;
using GRYLibrary.Core.ExecutePrograms;

namespace ConSurvBackend.Core.Misc
{
    /// <summary>
    /// Groups the data that belongs to a single active camera stream: the camera model, the
    /// associated external process, the port the stream is exposed on, and the stream path.
    /// </summary>
    internal class StreamOrganizationDataset
    {
      public  Camera Camera { get; set; }
        public ExternalProgramExecutor Process { get; set; }
        public ushort Port{ get; set; }
        public string Path{ get; set; }
    }
}
