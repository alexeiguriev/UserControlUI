using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class UsersController : Controller
    {
        UserAPI _api = new UserAPI();
        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = new List<UserDTO>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/User");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserDTO>>(result);
            }
            return View(users);
        }
    }
}
