using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConSurvBackend.Core.Services
{
    public class GeneralResourceLoader : GRYLibrary.Core.APIServer.Services.Res.GeneralResourceLoader
    {
        public GeneralResourceLoader() : base("ConSurvBackend.Core.Resources") { }
    }
}
