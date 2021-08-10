using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class HomeController : Controller
    {
        UserAPI _api = new UserAPI();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                UserDTO user = await GetUser();
                return View(user);
            }
            else
            {
                return View(null);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private async Task<UserDTO> GetUser()
        {
            UserDTO user = new UserDTO();
            HttpClient client = _api.Initial();

            // Get cookie from sesion
            string accessCokie = HttpContext.Session.GetString("JWToken");
            // Set client cookie
            client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // Get user Id from sesion
            int? id = HttpContext.Session.GetInt32("UserId");

            HttpResponseMessage res = await client.GetAsync("api/User/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserDTO>(result);
            }
            return user;

        }
    }
}
