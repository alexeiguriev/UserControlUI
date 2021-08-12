using HelperCSharp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient _client;
        public AuthenticationController(HttpClient client)
        {
            _client = client;
        }
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(UserInput loginInput,string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                UserDTO user = await GetUserIdentifyerFromServer(loginInput);

                ViewData["UserId"] = user.Id.ToString();

                var claims = new List<Claim>();
                claims.Add(new Claim("username", loginInput.EmailAddress));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, loginInput.EmailAddress));
                claims.Add(new Claim(ClaimTypes.Name, $"{ user.FirstName }  { user.LastName }"));
                foreach(string role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

                return Redirect(returnUrl);
            }
            catch
            {

            }
            TempData["Error"] = "Error. Username or Password is invalid";
            //return BadRequest();
            return View("login");

        }
        public async Task<ActionResult> Logout()
        {
            // Initialization.
            HttpResponseMessage response = new HttpResponseMessage();

            // HTTP POST
            response = await _client.GetAsync("api/logout");

            // Verification
            if (response.IsSuccessStatusCode)
            {
                // Clear auth object
                HttpContext.Session.Clear();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(UserInput user)
        {
            user.Password = Crypt.EncodePasswordToBase64(user.Password);

            // Setting content type.                   
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.  
            HttpResponseMessage response = new HttpResponseMessage();

            //Set Cookie
            string accessCokie = ViewData["JWToken"] as string;

            // Add cookie to client
            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // HTTP POST
            response = await _client.PostAsJsonAsync("api/User", user).ConfigureAwait(false);

            // Verification  
            if (response.IsSuccessStatusCode)
            {
                // Reading Response.  
                string result = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Login", "Authentication");
        }
        private async Task<UserDTO> GetUserIdentifyerFromServer(UserInput loginInput)
        {
            UserDTO user = null;
            try
            {
                loginInput.Password = Crypt.EncodePasswordToBase64(loginInput.Password);

                // Setting content type.
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialization.
                HttpResponseMessage response = new HttpResponseMessage();

                // HTTP POST
                response = await _client.PostAsJsonAsync("api/login", loginInput).ConfigureAwait(false);

                // Verification
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.
                    string result = response.Content.ReadAsStringAsync().Result;

                    //Get cookie
                    string cookie = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.First();

                    //get user
                    user = JsonConvert.DeserializeObject<UserDTO>(result);

                    ViewData["JWToken"] = cookie;

                }
                else
                {
                    throw new AuthenticationException("Authentication failed");
                }
            }
            catch
            {
                throw new AuthenticationException("Authentication failed");
            }
            return user;
        }
    }
}
