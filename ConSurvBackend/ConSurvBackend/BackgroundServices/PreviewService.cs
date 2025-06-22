using ConSurvBackend.Core.Constants;
using ConSurvBackend.Core.Services;
using GRYLibrary.Core.APIServer.BaseServices;
using GRYLibrary.Core.APIServer.Settings;
using GRYLibrary.Core.Logging.GRYLogger;
using System;
using System.Collections.Concurrent;

namespace ConSurvBackend.Core.BackgroundServices
{
    public class PreviewService : IteratingBackgroundService, IPreviewService
    {
        private readonly ICameraService _CameraService;
        private readonly IRTSPManager _RTSPManager;
        private readonly IGRYLog _Log;
        private readonly ConcurrentDictionary<string, byte[]> _PreviewPictures;
        public PreviewService(IApplicationConstants<CodeUnitSpecificConstants> constants, ICameraService cameraService, IGRYLog logger, IRTSPManager rtspManager, IGRYLog gryLog) : base(constants.ExecutionMode, logger)
        {
            this.Enabled = true;
            this.AdditionalDelay = TimeSpan.FromSeconds(10);
            this._CameraService = cameraService;
            this._RTSPManager = rtspManager;
            this._Log = gryLog;
            _PreviewPictures = new ConcurrentDictionary<string, byte[]>();
        }

        public byte[] GetPreview(string cameraId)
        {
            lock (cameraId)
            {
                if (!this._PreviewPictures.ContainsKey(cameraId))
                {
                    //TODO insert dummy-picture
                }
                return this._PreviewPictures[cameraId];
            }
        }

        protected override void Run()
        {
            foreach (var kvp in this._CameraService.GetAllCameras())
            {
                var camera = kvp.Value;
                lock (camera.Id)
                {
                    _Log.Log($"Start calculating preview for camera {camera.Id}...", Microsoft.Extensions.Logging.LogLevel.Debug);
                    try
                    {
                        this._PreviewPictures[camera.Id] = this._RTSPManager.GetPreviewDirectlyFromCamera(camera, 1280, 720, true, this._Log).picture;
                        _Log.Log($"Finished calculating preview for camera {camera.Id}...", Microsoft.Extensions.Logging.LogLevel.Debug);
                    }
                    catch(Exception e)
                    {
                        _Log.Log($"Error while calculating preview for camera {camera.Id}...",e);
                    }
                }
            }
        }
    }
}
