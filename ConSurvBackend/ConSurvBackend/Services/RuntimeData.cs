using ConSurvBackend.Core.Model;
using ConSurvBackend.Core.Model.Internals;
using GRYLibrary.Core.APIServer.Services.Interfaces;
using GRYLibrary.Core.APIServer.Services.Res;
using GRYLibrary.Core.Misc;
using System;
using System.Collections.Generic;

namespace ConSurvBackend.Core.Services
{
    public class RuntimeData : IRuntimeData
    {
        private readonly byte[] _NoPreviewAvailablePicture;
        private readonly ITimeService _TimeService;
        public RuntimeData(IGeneralResourceLoader generalResourceLoader, ITimeService timeService)
        {
            this._TimeService = timeService;
            this._NoPreviewAvailablePicture = generalResourceLoader.GetResource("NoPreviewAvailablePicture.jpg");
        }
        #region Camera Internals
        internal static readonly object CameraInternalsRuntimeDataLock = new object();
        private readonly Dictionary<string/*camera-id*/, CameraInternalsBase> _CameraInternals = new Dictionary<string, CameraInternalsBase>();
        public IDictionary<string, CameraInternalsBase> GetCameraInternals()
        {
            lock (CameraInternalsRuntimeDataLock)
            {
                return new Dictionary<string, CameraInternalsBase>(this._CameraInternals);
            }
        }

        public CameraInternalsBase GetCameraInternals(string cameraId)
        {
            lock (CameraInternalsRuntimeDataLock)
            {
                return this._CameraInternals[cameraId];
            }
        }

        public void SetCameraInternals(CameraInternalsBase cameraInternals)
        {
            lock (CameraInternalsRuntimeDataLock)
            {
                this._CameraInternals[cameraInternals.Camera.Id] = cameraInternals;
            }
        }
        public bool InternalsAreAvailable(string cameraId)
        {
            lock (CameraInternalsRuntimeDataLock)
            {
                return this._CameraInternals.ContainsKey(cameraId);
            }
        }

        public byte[] GetPreviewFallbackPicture()
        {
            return this._NoPreviewAvailablePicture;
        }
        #endregion

        #region Preview
        internal static readonly object PreviewRuntimeDataLock = new object();
        private readonly Dictionary<string/*camera-id*/, FixedSizeQueue<Preview>> _Previews = new Dictionary<string, FixedSizeQueue<Preview>>();

        public void AddPreview(string cameraId, Preview preview)
        {
            lock (PreviewRuntimeDataLock)
            {
                this.EnsurePreviewQueueIsAvailable(cameraId);
                this._Previews[cameraId].Enqueue(preview);
            }
        }

        public Preview GetLatestPreview(string cameraId)
        {
            lock (PreviewRuntimeDataLock)
            {
                this.EnsurePreviewQueueIsAvailable(cameraId);
                this.EnsureQueueIsNotEmpty(cameraId);
                return this._Previews[cameraId].GetEntries()[0];
            }
        }

        public IList<Preview> GetLatestPreviews(string cameraId)
        {
            lock (PreviewRuntimeDataLock)
            {
                this.EnsurePreviewQueueIsAvailable(cameraId);
                this.EnsureQueueIsNotEmpty(cameraId);
                return this._Previews[cameraId].GetEntries();
            }
        }

        private void EnsureQueueIsNotEmpty(string cameraId)
        {
            if (this._Previews[cameraId].Count == 0)
            {
                this._Previews[cameraId].Enqueue(new Preview(this._NoPreviewAvailablePicture, this._TimeService.GetCurrentLocalTimeAsDateTimeOffset()));
            }
        }

        private void EnsurePreviewQueueIsAvailable(string cameraId)
        {
            if (!this._Previews.ContainsKey(cameraId))
            {
                this._Previews[cameraId] = new FixedSizeQueue<Preview>(10);
            }
        }
        #endregion
    }
}
