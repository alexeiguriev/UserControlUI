using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.Intercafes;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class UsersController : Controller
    {
        private readonly JWTSettings _jwtsettings;
        private readonly HttpClient _client;
        public UsersController(IOptions<JWTSettings> jwtsettings, HttpClient client)
        {
            _jwtsettings = jwtsettings.Value;
            _client = client;
        }


        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = null;
            try
            {
                users = await GetUsers();
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
                users = await GetUsers();
            }
            catch
            {
                BadRequest(null);
            }
            return Ok(users);

        }
        public async Task<List<UserDTO>> GetUsers()
        {
            List<UserDTO> users = null;
            try
            {
                string accessCokie = ViewData["JWToken"] as string;
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

    }
}
