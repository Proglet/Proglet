using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models.Frontend.Login;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Frontend
{
    [Route("[controller]")]
    [Controller]
    public class LoginController : Controller
    {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }


        public IActionResult Index()
        {
            return View(loginService.GetLoginMethods());
        }
        
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpGet("Start/{name}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Start(string name)
        {
            var loginMethods = loginService.GetLoginMethods();
            var method = loginMethods.FirstOrDefault(m => m.Name == name);
            if (method == null)
                throw new Exception("Login method not found");
            
            switch(method.Type)
            {
                case "OAuth":
                    var bodyData = new
                    {
                        @return = $"{Request.Scheme}://{Request.Host}/Login/Finish/{method.Name}"
                    };
                    var res = loginService.HandleLogin(name, JsonDocument.Parse(JsonSerializer.Serialize(bodyData)).RootElement, Request);
                    return View(new Start()
                    {
                        Url = res.url,
                        Jwt = res.jwt
                    });
                    break;
                default:
                    throw new Exception("Login method not supported on website yet");
            }
            return View();
        }


        [HttpGet("Finish/{loginservice}")]
        public IActionResult Finish(string loginservice, string oauth_token, string oauth_verifier)
        {
            try
            {
                var finishData = loginService.FinishOAuth(new Models.API.OauthFinishData()
                {
                    jwt = Request.Cookies["jwt"],
                    loginservice = loginservice,
                    oauth_token = oauth_token,
                    oauth_verifier = oauth_verifier
                });
                return View("Finish", finishData.jwt);
            }
            catch (Exception)
            {
                return View("Finish", "");
            }

        }

    }
}
