using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Authorization;
using WebApp.Domain;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SomeController : ControllerBase
    {
        [HttpGet("public")]
        public string Public() => "Public";

        [ApiKeyAuthenticationEndpointFilter]
        [HttpGet("secure")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public string Secure() => "Secure";

        [ApiKeyAuthenticationEndpointFilter]
        [HttpGet("api-key")]
        public string ApiKey() => "ApiKey";

        [HttpGet("privileged")]
        [Authorize(Policy = "Customer")]
        public string Customer() => "Privileged";

        [HttpGet("administrator")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public string Admin() => "Administrator";

        [HttpGet("courses")]
        //[Authorize(Policy = "CourseView", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HasPermission(Permissions.CourseView)]
        public string Courses() => "Courses";
    }

}
