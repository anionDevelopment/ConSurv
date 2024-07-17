using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace ContinuousSurveillanceBackend.Core.Controller
{
    [ApiController]
    [Route(GUtilities.EmptyString)]
    public class HomepageController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route(GUtilities.EmptyString)]
        public IActionResult GetHomePage()
        {
            return this.Redirect($"/{Constants.CodeUnitSpecificConstants.WebControllerRoute}/index.html");
        }
    }
}
