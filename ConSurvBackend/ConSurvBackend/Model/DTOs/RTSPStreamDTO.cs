using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConSurvBackend.Core.Model.DTOs
{
    public class RTSPStreamDTO:VideoTypeDTO
    {
        public string StreamURL { get; set; }
    }
}
