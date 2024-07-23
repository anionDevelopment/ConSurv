using ContinuousSurveillanceBackend.Core.Model;
using ContinuousSurveillanceBackend.Core.Model.RecordingModes;
using System;
using System.Collections.Generic;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace ContinuousSurveillanceBackend.Core.Services
{
    public sealed class TransientPersistence : IPersistence
    {
        private readonly IDictionary<string, Camera> _Cameras = new Dictionary<string, Camera>();
        public string CreateCamera(string name, NoRecording notRecording)
        {
            var newCamera = new Camera()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                RecordingMode = notRecording,

            };
            this._Cameras.Add(newCamera.Id, newCamera);
            return newCamera.Id;
        }

        public void RemoveCamera(string cameraId)
        {
            this._Cameras.Remove(cameraId);
        }
        public void UpdateCamera(string cameraId, string name, RecordMode recordMode)
        {
            var updatedCamera = new Camera()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                RecordingMode = recordMode,

            };
            this._Cameras[cameraId] = updatedCamera;
        }

        public void Dispose()
        {
            GUtilities.NoOperation();
        }

        public bool IsAvailable()
        {
            throw new System.NotImplementedException();
        }
    }
}
