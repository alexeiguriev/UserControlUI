using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserControlUI.Models;

namespace UserControlUI.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        readonly private HttpClient _client;
        public RoleController(HttpClient client)
        {
            _client = client;
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            RoleDTO user = null;
            try
            {
                // Get cookie from sesion
                string accessCokie = ViewData["JWToken"] as string;
                // Set client cookie
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Role/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<RoleDTO>(result);
                }
            }
            catch
            {
                BadRequest(null);
            }            
            return Ok(user);
        }
    }
}
