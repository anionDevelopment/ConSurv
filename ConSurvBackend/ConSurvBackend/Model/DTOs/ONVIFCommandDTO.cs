using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.MoveDirections;
using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.ZoomDirections;
using System;

namespace ConSurvBackend.Core.Model.DTOs
{
    /// <summary>
    /// Data transfer object used to deserialize an ONVIF PTZ command received from a client.
    /// The <see cref="CommandType"/> discriminator selects the concrete <see cref="ONVIFCommand"/> subtype.
    /// </summary>
    public class ONVIFCommandDTO
    {
        public string CommandType { get; set; }
        public string Direction { get; set; }

        /// <summary>
        /// Converts this DTO into the appropriate <see cref="ONVIFCommand"/> subtype
        /// based on the value of <see cref="CommandType"/>.
        /// </summary>
        /// <returns>A concrete <see cref="ONVIFCommand"/> instance matching <see cref="CommandType"/>.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when <see cref="CommandType"/> does not correspond to a known command type.
        /// </exception>
        public ONVIFCommand ToONVIFCommand()
        {
            return this.CommandType switch
            {
                nameof(Move) => this.LoadMove(),
                nameof(Zoom) => this.LoadZoom(),
                _ => throw new NotSupportedException($"Unsupported {nameof(this.CommandType)}: '{this.CommandType}'"),
            };
        }

        private Move LoadMove()
        {
            return new Move
            {
                MoveDirection = this.Direction switch
                {
                    nameof(MoveUp) => (MoveDirection)new MoveUp(),
                    nameof(MoveDown) => new MoveDown(),
                    nameof(MoveLeft) => new MoveLeft(),
                    nameof(MoveRight) => new MoveRight(),
                    _ => throw new NotSupportedException($"Unsupported move direction: '{this.Direction}'"),
                }
            };
        }

        private Zoom LoadZoom()
        {
            return new Zoom
            {
                ZoomDirection = this.Direction switch
                {
                    nameof(ZoomIn) => (ZoomDirection)new ZoomIn(),
                    nameof(ZoomOut) => new ZoomOut(),
                    _ => throw new NotSupportedException($"Unsupported zoom direction: '{this.Direction}'"),
                }
            };
        }
    }
}
