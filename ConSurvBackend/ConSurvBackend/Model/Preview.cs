using System;

namespace ConSurvBackend.Core.Model
{
    public class Preview
    {

        public byte[] Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Preview(byte[] data, DateTimeOffset timestamp)
        {
            this.Timestamp = timestamp;
            this.Data = data;
        }
    }
}
