using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace Auth.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        public DocumentController(IMapper mapper, HttpClient client)
        {
            _client = client;
            _mapper = mapper;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<DocumentDTO> documents = new List<DocumentDTO>();

            // Set token
            string accessCokie = HttpContext.Session.GetString("JWToken");
            //accessCokie = ViewData["JWToken"] as string;
            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // Get Documents
            HttpResponseMessage res = await _client.GetAsync("api/Document");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);

                foreach(DocumentDTO document in documents)
                {
                    // Get user who updated this document
                    UserDTO user = await GetUserByIdAsync(document.UpdatedByUserId);

                    // Ser user name who updated in document structure.
                    document.UpdatedByUserName = $"{user.FirstName} {user.LastName}";
                }

            }
            return View(documents);
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            DocumentDTO document = await GetDocumentByIdAsync(id);
            return View(document);
        }
        [Authorize(Roles = "Admin,PM,Developer")]
        public IActionResult Create()
        {
            return View();
        }
        //Create document methode
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            // Convert iFormFile to InputDocument
            InputDocument document = UploadFile(file);

            // Set token
            string accessCokie = HttpContext.Session.GetString("JWToken");
            //string accessCokie = ViewData["JWToken"] as string;

            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // HTTP POST
            var response = await _client.PostAsJsonAsync("api/Document", document).ConfigureAwait(false);

            // Check if success
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                DocumentDTO documentDTO = JsonConvert.DeserializeObject<DocumentDTO>(result);
                await SendNotificationToAdminAndPm(documentDTO);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Authentication");
        }
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Edit()
        {
            return View();
        }
        //Create document methode
        [HttpPut]
        public async Task<IActionResult> Edit(IFormFile file, int id)
        {
            // Convert iFormFile to InputDocument
            InputDocument document = UploadFile(file);

            // Set token
            string accessCokie = HttpContext.Session.GetString("JWToken");
            //string accessCokie = ViewData["JWToken"] as string;

            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // HTTP POST
            var response = await _client.PostAsJsonAsync("api/Document", document).ConfigureAwait(false);

            // Check if success
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Authentication");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoc()
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("DocToDeleteId");

                // Set token
                string accessCokie = HttpContext.Session.GetString("JWToken");
                //string accessCokie = ViewData["JWToken"] as string;

                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                // HTTP POST
                var response = await _client.DeleteAsync("api/Document/" + id);

                // Check if success
                if (response.IsSuccessStatusCode)
                {

                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            // Store Doc Id
            HttpContext.Session.SetInt32("DocToDeleteId", id);

            // Get document by Id
            DocumentDTO document = await GetDocumentByIdAsync(id);

            return View(document);
        }
        public async Task<IActionResult> Dwonload(int id)
        {

            DocumentDTO doc = await GetFileFromDbByIdAsync(id);

            IActionResult file = File(doc.Content, doc.Type, doc.Name);
            return file;

        }
        private async Task<DocumentDTO> GetFileFromDbByIdAsync(int id)
        {
            DocumentDTO doc;
            try
            {
                // Set token
                string accessCokie = HttpContext.Session.GetString("JWToken");
                //string accessCokie = ViewData["JWToken"] as string;

                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                // HTTP POST
                var res = await _client.GetAsync("api/Document/" + id);

                // Check if success
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    doc = JsonConvert.DeserializeObject<DocumentDTO>(result);
                }
                else
                {
                    doc = null;
                }
            }
            catch
            {
                doc = null;
            }
            return doc;
        }
        public InputDocument UploadFile(IFormFile file)
        {
            InputDocument document;
            try
            {
                if (file != null)
                {
                    // Check if file is in range
                    if ((file.Length > 0) || (file.Length < 10000000))
                    {
                        // Convert input data to InputDocument data type
                        document = _mapper.Map<InputDocument>((file));

                        // Set status
                        document.Status = 1;

                        // Get user Id from sesion
                        int? id = HttpContext.Session.GetInt32("UserId");

                        // Set user Id
                        document.UpdaterId = (int)id;
                    }
                    else
                    {
                        document = null;
                    }
                }
                else
                {
                    document = null;
                }

            }
            catch
            {
                document = null;
            }
            return document;
        }
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            UserDTO user = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/User/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserDTO>(result);
                }
            }
            catch
            {
            }
            return user;
        }
        public async Task<DocumentDTO> GetDocumentByIdAsync(int id)
        {
            DocumentDTO document = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Document/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    document = JsonConvert.DeserializeObject<DocumentDTO>(result);

                    // Get user who updated this document
                    UserDTO user = await GetUserByIdAsync(document.UpdatedByUserId);

                    // Ser user name who updated in document structure.
                    document.UpdatedByUserName = $"{user.FirstName} {user.LastName}";
                }
            }
            catch
            {
            }
            return document;
        }
        public async Task<List<RoleDTO>> GetRolesAsync()
        {
            List<RoleDTO> roles = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Role");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    roles = JsonConvert.DeserializeObject<List<RoleDTO>>(result);
                }
            }
            catch
            {
            }
            return roles;
        }
        private int GetActualUserId()
        {
            int actualUserId = Int32.Parse(((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Sid)
                .Select(c => c.Value).ToArray()[0]);

            return actualUserId;
        }
        private async Task<List<int>> GetAdminandPmUserIds()
        {

            List<RoleDTO> allRoles = await GetRolesAsync();
            List<RoleDTO> actualRoles = new List<RoleDTO>();

            actualRoles.Add(allRoles.Find(ur => ur.Name == "Admin"));
            actualRoles.Add(allRoles.Find(ur => ur.Name == "PM"));

            List<int> userIds = new List<int>();
            foreach (RoleDTO role in actualRoles)
            {
                foreach (int userId in role.ReachedByUserIds)
                {
                    userIds.Add(userId);
                }
            }

            // Remove dublicates
            userIds = userIds.Distinct().ToList();
            return userIds;
        }
        public async Task SendNotificationToAdminAndPm(DocumentDTO document)
        {
            int actualUserId = GetActualUserId();
            Noti noti = new Noti()
            {
                NotiHeader = "Document added",
                NotiBody = $"Was Added a new document {document.Name}, by {User.Identity.Name}",
                FromUserId = actualUserId,
                IsRead = false,
                Url = $"../document/details/{document.Id}",
                Message = $"Was Added new document {document.Name}, by "
            };
            try
            {
                // Get Admin and PM userIds
                List<int> userIds = await GetAdminandPmUserIds();

                foreach (int userId in userIds)
                {
                    if (userId != actualUserId)
                    {
                        //string accessCokie = ViewData["JWToken"] as string;
                        string accessCokie = HttpContext.Session.GetString("JWToken");
                        _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                        // Add to user Id
                        noti.ToUserId = userId;

                        // HTTP POST
                        var res = await _client.PostAsJsonAsync("api/Noti", noti).ConfigureAwait(false);

                        // 
                        if (res.IsSuccessStatusCode)
                        {

                        }
                    }
                }
            }
            catch
            {
            }
            return;
        }


    }
}
