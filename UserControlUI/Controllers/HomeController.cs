using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, HttpClient client)
        {
            _client = client;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

            // Get cookie from sesion
            string accessCokie = ViewData["JWToken"] as string;
            // Set client cookie
            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // Get user Id from sesion
            int id = Int32.Parse(ViewData["UserId"] as string);


            HttpResponseMessage res = await _client.GetAsync("api/User/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserDTO>(result);
            }
            return user;

        }
    }
}
