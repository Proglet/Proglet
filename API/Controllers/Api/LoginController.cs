﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models.API;
using API.Services;
using API.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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


        /// <summary>
        /// Lists all available authentication systems
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpGet]
        public IActionResult GetLoginMethods()
        {
            return Ok(loginService.GetLoginMethods());
        }

        /// <summary>
        /// Logs in, using the specified login service
        /// </summary>
        /// <remarks>
        ///     POST login/login
        ///     {
        ///         "loginservice" : "avans"
        ///     }
        /// </remarks>
        [HttpPost("login")]
        public IActionResult Login([FromBody]JsonElement body)
        {
            var loginservice = body.GetProperty("loginservice").GetString();
            return Ok(loginService.HandleLogin(loginservice, body, Request));//TODO: not everything is ok
        }

        /// <summary>
        /// The return entry point for OAuth requests
        /// </summary>
        /// <param name="oauth_token"></param>
        /// <param name="oauth_verifier"></param>
        /// <returns></returns>
        [HttpGet("oauth")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        public IActionResult OAuth(string oauth_token, string oauth_verifier)
        {
            return Ok(new
            {
                oauth_token = oauth_token,
                oauth_verifier = oauth_verifier
            });
        }

        /// <summary>
        /// Finish up an OAuth login
        /// </summary>
        [HttpPost("oauthfinish")]
        public IActionResult OAuthFinish([FromBody]OauthFinishData finishData)
        {
            return Ok(loginService.FinishOAuth(finishData));
        }


        /// <summary>
        /// test API to see if the user is logged in properly
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("testlogin")]
        public IActionResult TestLogin()
        {
            return Ok("Logged in!");
        }


    }
}