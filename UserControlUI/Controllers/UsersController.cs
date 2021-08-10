using Microsoft.AspNetCore.Http;
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
        UserAPI _api = new UserAPI();
        private readonly JWTSettings _jwtsettings;
        public UsersController(IOptions<JWTSettings> jwtsettings)
        {
            _jwtsettings = jwtsettings.Value;
        }


        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = new List<UserDTO>();
            HttpClient client = _api.Initial();

            string accessCokie = HttpContext.Session.GetString("JWToken");
            client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            HttpResponseMessage res = await client.GetAsync("api/User");
            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Authentication");
            }
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserDTO>>(result);
            }
            return View(users);
        }
    }
}
