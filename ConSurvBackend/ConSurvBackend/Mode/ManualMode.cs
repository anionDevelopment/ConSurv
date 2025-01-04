namespace ConSurvBackend.Core.Mode
{
    internal class ManualMode : BaseMode
    {
        public string AdminPassword { get; private set; }
        public System.Collections.Generic.IEnumerable<string> CameraAddresses { get; private set; }

        public ManualMode(string adminPassword, System.Collections.Generic.IEnumerable<string> cameraAddresses)
        {
            this.AdminPassword = adminPassword;
            this.CameraAddresses = cameraAddresses;
        }
        #region Overhead
        public override void Accept(IBaseModeVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IBaseModeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        #endregion
    }
}
