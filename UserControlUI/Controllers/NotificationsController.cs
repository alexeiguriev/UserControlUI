using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class NotificationsController : Controller
    {
        Noti _noty = new Noti()
        {
            NotiId = 1,
            FromUserId = 1,
            ToUserId = 2,
            NotiHeader = "this is notyfication header",
            NotiBody = "This is notification body",
            IsRead = false,
            Url = "../login",
            CreatedDate = DateTime.Now,
            Message = "This is notification message",
            FromUserName = "AlexeiFrom",
            ToUserName = "AlexeiTo"

        };
        Noti _noty1 = new Noti()
        {
            NotiId = 1,
            FromUserId = 1,
            ToUserId = 2,
            NotiHeader = "this is notyfication header",
            NotiBody = "This is notification body",
            IsRead = false,
            Url = "../Authentication/Register",
            CreatedDate = DateTime.Now,
            Message = "This is notification message",
            FromUserName = "AlexeiFrom",
            ToUserName = "AlexeiTo"

        };
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllNotifications()
        {
            return View();
        }
        public JsonResult GetNotifications(bool getOnliUnreaded = false)
        {
            List<Noti> notiList = new List<Noti>();
            notiList.Add(_noty);
            notiList.Add(_noty1);

            JsonResult result = Json(notiList);

            return result;
        }
    }
}
