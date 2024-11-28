using ConSurvBackend.Core.Model.SpecialFunctions.ONVIF.Commands;
using System;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class ONVIFCommandDTO
    {
        public string CommandType {  get; set; }
        public ONVIFCommand ToONVIFCommand()
        {
            return this.CommandType switch
            {
                nameof(Move) => this.LoadMove(),
                nameof(Zoom) => this.LoadZoom(),
                _ => throw new NotSupportedException($"Unsupported {nameof(this.CommandType)}: '{this.CommandType}'"),
            };
        }

        private Move LoadZoom()
        {
            Move result = new Move();
            //TODO set properties
            return result;
        }

        private Zoom LoadMove()
        {
            Zoom result = new Zoom();
            //TODO set properties
            return result;
        }
    }
}
