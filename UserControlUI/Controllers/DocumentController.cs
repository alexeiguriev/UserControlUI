using AutoMapper;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly HttpClient _client;
        public DocumentController(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<DocumentDTO> documents = new List<DocumentDTO>();

            // Set token
            string accessCokie = ViewData["JWToken"] as string;
            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // Get Documents
            HttpResponseMessage res = await _client.GetAsync("api/Document");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);

            }
            return View(documents);
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

            // Set token
            string accessCokie = ViewData["JWToken"] as string;
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
                        int id = Int32.Parse(ViewData["UserId"] as string);

                        // Set user Id
                        document.UpdaterId = id;
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

        //private async Task<UserDTO> GetUser()
        //{
        //    UserDTO user = new UserDTO();
        //    HttpClient client = _api.Initial();

        //    // Get cookie
        //    string accessCokie = ViewData["JWToken"] as string;

        //    // Get user Id from sesion
        //    int id = Int32.Parse(ViewData["UserId"] as string);

        //    HttpResponseMessage res = await client.GetAsync("api/User/" + id);
        //    if (res.IsSuccessStatusCode)
        //    {
        //        var result = res.Content.ReadAsStringAsync().Result;
        //        user = JsonConvert.DeserializeObject<UserDTO>(result);
        //    }
        //    return user;

        //}
    }

}
