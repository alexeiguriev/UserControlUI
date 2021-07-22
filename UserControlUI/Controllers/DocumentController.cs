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
    public class DocumentController : Controller
    {
        UserAPI _api = new UserAPI();
        public async Task<IActionResult> Index()
        {
            List<DocumentDTO> users = new List<DocumentDTO>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Document");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);
            }
            return View(users);
        }
        ////Create document methode
        //public async Task<IActionResult> Create()
        //{
        //    List<DocumentDTO> users = new List<DocumentDTO>();
        //    HttpClient client = _api.Initial();
        //    HttpResponseMessage res = await client.GetAsync("api/Document");
        //    if (res.IsSuccessStatusCode)
        //    {
        //        var result = res.Content.ReadAsStringAsync().Result;
        //        users = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);
        //    }
        //    return View(users);
        //}

    }
}
