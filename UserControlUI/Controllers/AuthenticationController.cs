using HelperCSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.Intercafes;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class AuthenticationController : Controller
    {
        UserAPI _api = new UserAPI();
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(UserInput loginInput)
        {
            loginInput.Password = Crypt.EncodePasswordToBase64(loginInput.Password);

            HttpClient client = _api.Initial();

            // Setting content type.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.
            HttpResponseMessage response = new HttpResponseMessage();

            // HTTP POST
            response = await client.PostAsJsonAsync("api/login", loginInput).ConfigureAwait(false);

            // Verification
            if (response.IsSuccessStatusCode)
            {
                // Reading Response.
                string result = response.Content.ReadAsStringAsync().Result;

                //Get cookie
                string cookie = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.First();

                //get user
                UserDTO user = JsonConvert.DeserializeObject<UserDTO>(result);

                HttpContext.Session.SetString("JWToken", cookie);

                HttpContext.Session.SetInt32("UserId", user.Id);

                

                // Reading Response.
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Login", "Authentication");
        }
        public async Task<ActionResult> Logout()
        {
            HttpClient client = _api.Initial();

            // Initialization.
            HttpResponseMessage response = new HttpResponseMessage();

            // HTTP POST
            response = await client.GetAsync("api/logout");

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
            HttpClient client = _api.Initial();

            // Setting content type.                   
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.  
            HttpResponseMessage response = new HttpResponseMessage();

            //Set Cookie
            string accessCokie = HttpContext.Session.GetString("JWToken");

            // Add cookie to client
            client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // HTTP POST
            response = await client.PostAsJsonAsync("api/User", user).ConfigureAwait(false);


            // Verification  
            if (response.IsSuccessStatusCode)
            {
                // Reading Response.  
                string result = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Login", "Authentication");
        }
    }
}
