using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IMapper _mapper;
        public DocumentController(IMapper mapper)
        {
            _mapper = mapper;
         }
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
        public IActionResult Create()
        {
            return View();
        }
        //Create document methode
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            //try
            //{
            //    if (file.ContentLength > 0)
            //    {
            //        string _FileName = Path.GetFileName(file.FileName);
            //        string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
            //        file.SaveAs(_path);
            //    }
            //    ViewBag.Message = "File Uploaded Successfully!!";
            //    return View();
            //}
            //catch
            //{
            //    ViewBag.Message = "File upload failed!!";
            //    return View();
            //}
            return View();
        }
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            if (file != null)
            {
                // Check if file is in range
                if ((file.Length > 0) || (file.Length < 10000000))
                {
                    // Convert input data to InputDocument data type
                    InputDocument storageDocument = _mapper.Map<InputDocument>((inputJSONString, file));

                    // Put on db new document
                    var newDocument = await _uof.DocumentRepository.Create(storageDocument);

                    // Map data convertion
                    DocumentDTO documentDTO = _mapper.Map<DocumentDTO>(newDocument);

                    // Log the post data information
                    _logger.LogInformation($"Post document: Name: {documentDTO.Name} ");

                    //Return statuc
                    return Ok(documentDTO);
                }
                else
                {
                    // File out of range
                    return NotFound("File length out of range");
                }
            }
            else
            {
                // File object is null
                return NotFound("Wrong file: file is null");
            }
        }

    }
}
