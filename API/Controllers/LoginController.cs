using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Services;
using API.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }


        // GET: api/Login
        [HttpGet]
        public IActionResult GetLoginMethods()
        {
            return Ok(loginService.GetLoginMethods());
        }

        /// <summary>
        /// POST: api/login/login
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody]JsonElement body)
        {
            var loginservice = body.GetProperty("loginservice").GetString();
            return Ok(loginService.HandleLogin(loginservice, body, Request));//TODO: not everything is ok
        }

        [HttpGet("oauth")]
        public IActionResult OAuth(string oauth_token, string oauth_verifier)
        {
            return Ok(new
            {
                oauth_token = oauth_token,
                oauth_verifier = oauth_verifier
            });
        }



        public class OauthFinishData
        {
            public string oauth_token { get; set; }
            public string oauth_verifier { get; set; }
            public string jwt { get; set; }
        }
        [HttpPost("oauthfinish")]
        public IActionResult OAuthFinish([FromBody]OauthFinishData finishData)
        {

            return Ok("Ok");
        }



    }
}