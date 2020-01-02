using API.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace API.Services
{
    public interface ILoginService
    {
        List<string> GetLoginMethods();

        //TODO: don't send the request along, it's just needed for OAuth return url
        object HandleLogin(string loginservice, JsonElement body, HttpRequest request);
    }


    class SessionManager
    {
        private Dictionary<Guid, string> sessions = new Dictionary<Guid, string>();
        public Guid NewSession()
        {
            Guid newId = Guid.NewGuid();
            while (sessions.ContainsKey(newId))
                newId = Guid.NewGuid();
            sessions[newId] = "";
            return newId;
        }

        public string this[Guid id] {
            get {
                return sessions[id];
            }
            set {
                sessions[id] = value;
            }
        }
    }

    public class LoginService : ILoginService
    {
        private Settings.Login loginSettings;
        private Settings.Jwt jwtSettings;
        private SessionManager sessionManager = new SessionManager();

        public LoginService(IOptions<Login> loginSettings, IOptions<Jwt> jwtSettings)
        {
            this.loginSettings = loginSettings.Value;
            this.jwtSettings = jwtSettings.Value;
        }

        public List<string> GetLoginMethods()
        {
            return loginSettings.Keys.ToList();
        }

        public object HandleLogin(string loginservice, JsonElement body, HttpRequest request)
        {
            if (!loginSettings.ContainsKey(loginservice))
                return "Service not found";

            var login = loginSettings[loginservice];

            if (login.Type == "OAuth")
            {
                return HandleLoginOauth(login, body, request);
            }
            else
                return "Service type unknown";


        }

        private object HandleLoginOauth(LoginInfo login, JsonElement body, HttpRequest clientRequest)
        {
            
            OAuthRequest client = OAuthRequest.ForRequestToken(login.OAuth.ConsumerKey, login.OAuth.ConsumerSecret);
            client.RequestUrl = login.OAuth.RequestUrl;
            client.CallbackUrl = clientRequest.Scheme + "://" + clientRequest.Host + "/api/login/oauth"; //TODO: softcode this better?

            //make request for a token
            var url = client.RequestUrl + "?" + client.GetAuthorizationQuery();
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            string querystring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var query = HttpUtility.ParseQueryString(querystring);

            //store request in a temporary session
            var sessionid = sessionManager.NewSession();
            sessionManager[sessionid] = query["oauth_token_secret"];

            //store session ID in a JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("EtienneJohanJoep");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, sessionid.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new
            {
                result = "oauth",
                url = login.OAuth.LoginUrl + "?oauth_token=" + query["oauth_token"],
                jwt = tokenHandler.WriteToken(token)
            };
        }
    }
}
