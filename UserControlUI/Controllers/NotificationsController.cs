using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        public async Task<IActionResult> AllNotifications()
        {
            int actualUserId = GetActualUserId();

            List<Noti> notiList = await GetNotificationByUserToIdAsync(actualUserId);

            return View(notiList);
        }
        public async Task<JsonResult> GetNotifications(bool getOnliUnreaded = false)
        {
            int actualUserId = GetActualUserId();

            List<Noti> notiList = await GetNotificationByUserToIdAsync(actualUserId);
            
            // Mark notifications as reaad
            foreach(Noti noti in notiList)
            {
                await MarkNotificationAsReadByUserIdAsync(noti.Id);
            }
            JsonResult result = Json(notiList);

            return result;
        }
        public IActionResult Goto(string url)
        {
            return Redirect(url);
        }
        public async Task<IActionResult> Delete(int id)
        {
            // Delete the notification
            await DeleteNotification(id);

            return View();
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
        public async Task MarkNotificationAsReadByUserIdAsync(int id)
        {
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.PutAsJsonAsync("api/Noti/" + id + "/isread", id).ConfigureAwait(false);
                if (res.IsSuccessStatusCode)
                {
                    // Do nothing
                }
            }
            catch
            {
            }
            return;
        }
        public async Task<bool> DeleteNotification(int id)
        {
            bool success = false;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.DeleteAsync("api/Noti/" + id);
                if (res.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch
            {
            }
            return success;
        }
        private int GetActualUserId()
        {
            int actualUserId = Int32.Parse(((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Sid)
                .Select(c => c.Value).ToArray()[0]);

            return actualUserId;
        }
    }
}
