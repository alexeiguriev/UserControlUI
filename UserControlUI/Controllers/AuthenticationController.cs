using HelperCSharp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace Auth.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient _client;
        public AuthenticationController(HttpClient client)
        {
            _client = client;
        }
        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            return View();
        }
        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password,string returnUrl)
        {
            UserInput loginInput = new UserInput()
            {
                EmailAddress = username,
                Password = password
            };
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                UserDTO user = await GetUserIdentifyerFromServer(loginInput);

                ViewData["UserId"] = user.Id.ToString();

                var claims = new List<Claim>();
                claims.Add(new Claim("username", loginInput.EmailAddress));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, loginInput.EmailAddress));
                claims.Add(new Claim(ClaimTypes.Name, $"{ user.FirstName }  { user.LastName }"));
                foreach (string role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

                if(string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect("Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            catch
            {

            }
            TempData["Error"] = "Error. Username or Password is invalid";
            //return BadRequest();
            return View("login");
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

                    HttpContext.Session.SetString("JWToken", cookie);

                    HttpContext.Session.SetInt32("UserId", user.Id);

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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(UserInput user)
        {
            // Get all user roles
            List<RoleDTO> allUserRoles = await GetRolesAsync();

            // Set first in list user role for this new user
            user.UserRolesId = new int[] { allUserRoles[0].Id };

            user.Password = Crypt.EncodePasswordToBase64(user.Password);

            // Setting content type.                   
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.  
            HttpResponseMessage response = new HttpResponseMessage();

            //Set Cookie
            string accessCokie = HttpContext.Session.GetString("JWToken");

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
        public async Task<List<RoleDTO>> GetRolesAsync()
        {
            List<RoleDTO> roles = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Role");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    roles = JsonConvert.DeserializeObject<List<RoleDTO>>(result);
                }
            }
            catch
            {
            }
            return roles;
        }
    }
}
