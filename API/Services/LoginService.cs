using API.Controllers;
using API.Models;
using API.Settings;
using CoreDataORM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth;
using Proglet.Core.Data;
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
        List<LoginMethod> GetLoginMethods();

        //TODO: don't send the request along, it's just needed for OAuth return url
        object HandleLogin(string loginservice, JsonElement body, HttpRequest request);
        object FinishOAuth(OauthFinishData finishData);
    }



    public class LoginService : ILoginService
    {
        private Settings.Login loginSettings;
        private Settings.Jwt jwtSettings;
        private DataContext dataContext;
        private LoginOauthSessionService sessionManager;
        private OAuthRequest validatedClient;

        public LoginService(IOptions<Login> loginSettings, IOptions<Jwt> jwtSettings, LoginOauthSessionService sessionManager, DataContext dataContext)
        {
            this.dataContext = dataContext;
            this.sessionManager = sessionManager;
            this.loginSettings = loginSettings.Value;
            this.jwtSettings = jwtSettings.Value;
        }

       

        public List<LoginMethod> GetLoginMethods()
        {
            return loginSettings.Select(kv => new LoginMethod() { Name = kv.Key, Type = kv.Value.Type }).ToList();
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
            var returnAddress = body.GetProperty("return").GetString();

            OAuthRequest client = OAuthRequest.ForRequestToken(login.OAuth.ConsumerKey, login.OAuth.ConsumerSecret);
            client.RequestUrl = login.OAuth.RequestUrl;
            client.CallbackUrl = returnAddress;

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
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, sessionid.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretPreLogin)), SecurityAlgorithms.HmacSha256Signature)
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
            ValidateToken(finishData.jwt, jwtSettings.SecretPreLogin);

            var token = tokenHandler.ReadJwtToken(finishData.jwt);

            Guid sessionId = Guid.Parse((string)token.Payload[ClaimTypes.Sid]);
            if (!sessionManager.hasSession(sessionId))
                return "Session not found";
            string token_secret = sessionManager[sessionId];
            //TODO: delete the secret


            //confirm authenticity
            var client = OAuthRequest.ForRequestToken(login.OAuth.ConsumerKey, login.OAuth.ConsumerSecret);
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
            this.validatedClient = OAuthRequest.ForRequestToken(login.OAuth.ConsumerKey, login.OAuth.ConsumerSecret);
            this.validatedClient.RequestUrl = login.OAuth.InfoUrl;
            this.validatedClient.Token = oauth_token;
            this.validatedClient.TokenSecret = oauth_token_secret;

            //make request
            url = this.validatedClient.RequestUrl + "?" + this.validatedClient.GetAuthorizationQuery();
            request = (HttpWebRequest)WebRequest.Create(url);
            response = (HttpWebResponse)request.GetResponse();
            string info = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JsonElement infoJson = JsonSerializer.Deserialize<JsonElement>(info);
            string loginName = infoJson[0].GetProperty("inlognaam").GetString();
            int id = int.Parse(infoJson[0].GetProperty("studentnummer").GetString());

            var user = CheckOauthLogin(finishData.loginservice, id);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim("client_id", user.UserId+"")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var newToken = tokenHandler.CreateToken(tokenDescriptor);

            return new
            {
                jwt = tokenHandler.WriteToken(newToken)
            };
        }

        private User CheckOauthLogin(string loginservice, int id)
        {
            var oauthLogin = dataContext.OauthLogins.Where(l => l.LoginService == loginservice && l.OauthLoginId == id).Include(l => l.User).FirstOrDefault();
            if(oauthLogin == null) // no user made yet
            {
                validatedClient.RequestUrl = loginSettings[loginservice].OAuth.MoreInfoUrl;

                var url = validatedClient.RequestUrl + "?" + validatedClient.GetAuthorizationQuery();
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                string info = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Console.WriteLine(info);
                JsonElement infoJson = JsonSerializer.Deserialize<JsonElement>(info);

                string fullName = infoJson.GetProperty("name").GetProperty("formatted").GetString();
                string email = infoJson.GetProperty("emails")[0].GetString();
                bool isTeacher = infoJson.GetProperty("employee").GetString() == "true";
                bool isStudent = infoJson.GetProperty("student").GetString() == "true";

                User user = new User()
                {
                    Username = fullName,
                    Email = email,
                    OrganizationIdentifier = id+"",
                    UserRole = isTeacher ? UserRoles.Teacher : UserRoles.Student,
                    RegistrationDate = DateTime.Now
                };
                dataContext.Users.Add(user);
                dataContext.SaveChanges();


                oauthLogin = new OauthLogin()
                {
                    OauthLoginId = id,
                    LoginService = loginservice,
                    User = user
                };

                dataContext.OauthLogins.Add(oauthLogin);
                dataContext.SaveChanges();
                user.OauthLogin = oauthLogin;
                dataContext.SaveChanges();
            }
            return oauthLogin.User;
        }

        private static JwtSecurityToken ValidateToken(string jwt, string key)
        {
            var validationParameters = new TokenValidationParameters
            {
                // Clock skew compensates for server time drift.
                // We recommend 5 minutes or less:
                ClockSkew = TimeSpan.FromMinutes(5),
                // Specify the key used to sign the token:
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                RequireSignedTokens = true,
                // Ensure the token hasn't expired:
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = false, //TODO: change to true
                ValidateIssuer = false, //TODO: change to true
            };

            try {
                var claimsPrincipal = new JwtSecurityTokenHandler()
                    .ValidateToken(jwt, validationParameters, out var rawValidatedToken);
                return (JwtSecurityToken)rawValidatedToken;
            }
            catch (SecurityTokenValidationException stvex) {
                throw new Exception($"Token failed validation: {stvex.Message}");
            }
            catch (ArgumentException argex) {
                throw new Exception($"Token was invalid: {argex.Message}");
            }
        }
    }
}
