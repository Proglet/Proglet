using API.Controllers;
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
using static API.Controllers.LoginController;

namespace API.Services
{
    public interface ILoginService
    {
        List<string> GetLoginMethods();

        //TODO: don't send the request along, it's just needed for OAuth return url
        object HandleLogin(string loginservice, JsonElement body, HttpRequest request);
        object FinishOAuth(OauthFinishData finishData);
    }



    public class LoginService : ILoginService
    {
        private Settings.Login loginSettings;
        private Settings.Jwt jwtSettings;
        private LoginOauthSessionService sessionManager;

        public LoginService(IOptions<Login> loginSettings, IOptions<Jwt> jwtSettings, LoginOauthSessionService sessionManager)
        {
            this.sessionManager = sessionManager;
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
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
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



        public object FinishOAuth(OauthFinishData finishData)
        {
            if (!loginSettings.ContainsKey(finishData.loginservice))
                return "Service not found";
            var login = loginSettings[finishData.loginservice];

            //get the secret from the session manager
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(finishData.jwt);
            //TODO: validate JWT
            Guid sessionId = Guid.Parse((string)token.Payload[ClaimTypes.Sid]);
            if (!sessionManager.hasSession(sessionId))
                return "Session not found";
            string token_secret = sessionManager[sessionId];
            //TODO: delete the secret


            //confirm authenticity
            OAuthRequest client = OAuthRequest.ForRequestToken(login.OAuth.ConsumerKey, login.OAuth.ConsumerSecret);
            client.RequestUrl = login.OAuth.TokenUrl;
            client.Token = finishData.oauth_token;
            client.TokenSecret = token_secret;
            client.Verifier = finishData.oauth_verifier;

            //make request
            var url = client.RequestUrl + "?" + client.GetAuthorizationQuery();
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            string querystring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var query = HttpUtility.ParseQueryString(querystring);
		    var oauth_token = query["oauth_token"];
		    var oauth_token_secret = query["oauth_token_secret"];


            //make a request for information
            client = OAuthRequest.ForRequestToken(login.OAuth.ConsumerKey, login.OAuth.ConsumerSecret);
            client.RequestUrl = login.OAuth.InfoUrl;
            client.Token = oauth_token;
            client.TokenSecret = oauth_token_secret;

            //make request
            url = client.RequestUrl + "?" + client.GetAuthorizationQuery();
            request = (HttpWebRequest)WebRequest.Create(url);
            response = (HttpWebResponse)request.GetResponse();
            querystring = new StreamReader(response.GetResponseStream()).ReadToEnd();



            return querystring;
        }
    }
}
