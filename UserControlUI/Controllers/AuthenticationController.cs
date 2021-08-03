using HelperCSharp;
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
using UserControlUI.Models;

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
        public async Task<ActionResult> Login(LoginUser loginUser)
        {
            loginUser.Password = Crypt.EncodePasswordToBase64(loginUser.Password);

            HttpClient client = _api.Initial();

            // Setting content type.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.
            HttpResponseMessage response = new HttpResponseMessage();

            // HTTP POST
            response = await client.PostAsJsonAsync("api/login", loginUser).ConfigureAwait(false);


            var cookieContainer = new CookieContainer();
            List<Cookie> cookies = cookieContainer.GetCookies(client.BaseAddress).Cast<Cookie>().ToList();


            // Verification
            if (response.IsSuccessStatusCode)
            {
                // Reading Response.
                string result = response.Content.ReadAsStringAsync().Result;

                // Reading Response.
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Login", "Authentication");
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
        //[HttpPost]
        //public async Task<ActionResult> Register(UserInput user)
        //{
        //    HttpClient client = _api.Initial();

        //    //var jsonObject = JsonConvert.Serialize<UserInput>(user);

        //    string jsonString = JsonConvert.SerializeObject(user);
        //    HttpContent content = new StringContent(jsonString.ToString());

        //    //HTTP POST
        //    HttpResponseMessage resp = await client.PostAsync("api/User", content);

        //    if (resp.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    return View(user);
        //}
    }
}
