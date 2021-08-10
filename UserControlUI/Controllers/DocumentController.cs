using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UserControlUI.Helper;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IMapper _mapper;
        public DocumentController(IMapper mapper)
        {
            _mapper = mapper;
         }
        UserAPI _api = new UserAPI();
        public async Task<IActionResult> Index()
        {
            DocumentUserDTO documentsUser = new DocumentUserDTO();
            List<DocumentDTO> documents = new List<DocumentDTO>();

            // Get client
            HttpClient client = _api.Initial();

            // Set token
            string accessCokie = HttpContext.Session.GetString("JWToken");
            client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // Get Documents
            HttpResponseMessage res = await client.GetAsync("api/Document");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                documentsUser.Document = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);
                documentsUser.User = await GetUser();
            }
            return View(documentsUser);
        }
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

            // Get client
            HttpClient client = _api.Initial();

            // Set token
            string accessCokie = HttpContext.Session.GetString("JWToken");
            client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // HTTP POST
            var response = await client.PostAsJsonAsync("api/Document", document).ConfigureAwait(false);

            // Check if success
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Authentication");
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
