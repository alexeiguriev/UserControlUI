using HelperCSharp;
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
using System.Threading.Tasks;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace Auth.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly HttpClient _client;
        public UserController( HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = null;
            try
            {
                users = await GetUsersAsync();
            }
            catch
            {

            }
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            List<UserDTO> users = null;
            try
            {
                users = await GetUsersAsync();
            }
            catch
            {
                BadRequest(null);
            }
            return Ok(users);

        }
        public async Task<List<UserDTO>> GetUsersAsync()
        {
            List<UserDTO> users = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/User");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<UserDTO>>(result);
                }
            }
            catch
            {
            }
            return users;
        }
        [HttpGet]
        public  async Task<IActionResult> Get(UserInput loginInput)
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
            return Ok(user);
        }
    }
}
