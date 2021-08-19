using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        public NotificationsController(IMapper mapper, HttpClient client)
        {
            _client = client;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllNotifications()
        {
            return View();
        }
        public async Task<JsonResult> GetNotifications(bool getOnliUnreaded = false)
        {
            int actualUserId = Int32.Parse(((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Sid)
                .Select(c => c.Value).ToArray()[0]);

            List<Noti> notiList = await GetNotificationByUserToIdAsync(actualUserId);
            
            JsonResult result = Json(notiList);

            return result;
        }
        public async Task<List<Noti>> GetNotificationByUserToIdAsync(int id)
        {
            List<Noti> notis = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Noti/" + id + "/byuserto");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    notis = JsonConvert.DeserializeObject<List<Noti>>(result);
                }
            }
            catch
            {
            }
            return notis;
        }
    }
}
