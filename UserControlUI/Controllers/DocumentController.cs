﻿using AutoMapper;
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

            }
            return View(documents);
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
        public IActionResult Delete(int id, string name, string type)
        {
            // Store Doc Id
            HttpContext.Session.SetInt32("DocToDeleteId", id);

            DocumentDTO document = new DocumentDTO()
            {
                Id = id,
                Name = name,
                Type = type
            };
            return View(document);
        }
        public async Task<IActionResult> Dwonload(int docId)
        {

            DocumentDTO doc = await GetFileFromDbByIdAsync(docId);

            FormFile formFile = DownloadFile(doc);

            return RedirectToAction("Index");

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
        public FormFile DownloadFile(DocumentDTO document)
        {
            using (var stream = new MemoryStream(document.Content))
            {
                var file = new FormFile(stream, 0, document.Content.Length, "file", document.Name)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = document.Type,
                };

                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName
                };
                file.ContentDisposition = cd.ToString();
            }
            throw new NotImplementedException();

        }
    }
    }
