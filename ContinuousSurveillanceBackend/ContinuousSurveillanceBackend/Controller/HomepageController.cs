using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;

namespace ContinuousSurveillanceBackend.Core.Controller
{
    [ApiController]
    [Route(GUtilities.EmptyString)]
    public class HomepageController : ControllerBase
    {
        /*
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route(GUtilities.EmptyString)]
        public IActionResult GetHomePage()
        {
            return this.Redirect($"/{Constants.CodeUnitSpecificConstants.WebControllerRoute}");
        }
        */
    }
}
