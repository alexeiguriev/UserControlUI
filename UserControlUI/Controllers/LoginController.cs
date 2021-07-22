using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.Models;

namespace UserControlUI.Controllers
{
    public class LoginController : Controller
    {
        UserAPI _api = new UserAPI();
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        //[ValidateAntiForgeryToken]
        //public IActionResult Register(UserInput obj)
        //{
        //    List<UserInput> users = new List<UserInput>();
        //    HttpClient client = _api.Initial();
        //    //HttpResponseMessage res = await client.GetAsync("api/User");
        //    //if (res.IsSuccessStatusCode)
        //    //{
        //    //    var result = res.Content.ReadAsStringAsync().Result;
        //    //    users = JsonConvert.DeserializeObject<List<UserDTO>>(result);
        //    //}
        //    //return View(obj);
        //    return View();

        //}
    }
}
