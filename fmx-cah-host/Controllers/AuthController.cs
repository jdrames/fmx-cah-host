using fmx_cah_host.Models.FormData;
using fmx_cah_host.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host.Controllers
{
    // TODO: Convert to Discord oauth 

    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;

        public AuthController(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserLoginPost user)
        {
            var userId = Nanoid.Nanoid.Generate();
            var token = _authService.CreateJwtToken(userId, user.Name);
            return Ok(new
            {
                id = userId,
                token = token,
                name = user.Name
            });
        }
    }
}
