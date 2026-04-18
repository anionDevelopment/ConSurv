namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object that encodes the current <see cref="RecordStates.RecordState"/> of a camera
    /// as its type name for transport over the API.
    /// </summary>
    public class RecordStateDTO
    {
        public string RecordState { get;  set; }
    }
}
