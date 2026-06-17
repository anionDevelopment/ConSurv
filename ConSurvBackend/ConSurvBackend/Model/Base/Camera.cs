using ConSurvBackend.Core.Model.RecordModes;
using System;

namespace ConSurvBackend.Core.Model.Base
{
    /// <summary>
    /// Represents a surveillance camera with its configuration, video source, and recording settings.
    /// </summary>
    public class Camera
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public VideoInformation VideoInformation { get; internal set; }
        public RecordMode RecordMode { get; internal set; }
        public bool Enabled { get; internal set; }
        public TimeZoneInfo TimeZone { get; internal set; } = TimeZoneInfo.Local;//TODO make this configurable
        public Overlay Overlay { get; internal set; } = new Overlay();

        /// <summary>
        /// Initializes a new <see cref="Camera"/> with the given identifier and name,
        /// using a default RTSP stream URL derived from the name, no TLS certificate,
        /// <see cref="RecordAlways"/> as the recording mode, and the camera enabled.
        /// </summary>
        /// <param name="id">The unique identifier of the camera.</param>
        /// <param name="name">The human-readable name of the camera.</param>
        public Camera(string id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.VideoInformation = new VideoInformation()
            {
                Certificate = null,
                SupportsPTZViaONVIF = false,
                StreamURL=$"rtsp://{name}"
            };
            this.RecordMode = new RecordAlways();
            this.Enabled = true;
        }

        /// <summary>
        /// Determines whether two <see cref="Camera"/> instances refer to the same camera
        /// by comparing their <see cref="Id"/> values.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Camera"/> with the same <see cref="Id"/>; otherwise <see langword="false"/>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is Camera camera &&
                   this.Id == camera.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }

        /// <inheritdoc />
        public override string? ToString()
        {
            return $"Camera_{this.Id}({this.Name})";
        }
    }
}
