using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        private readonly IUserService _userService;
        public DocumentService(HttpClient client, IMapper mapper, IUserService userService)
        {
            _client = client;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<List<DocumentDTO>> GetAllDocuments(string accessCokie)
        {
            List<DocumentDTO> documents = new List<DocumentDTO>();

            //accessCokie = ViewData["JWToken"] as string;
            _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

            // Get Documents
            HttpResponseMessage res = await _client.GetAsync("api/Document");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);

                foreach (DocumentDTO document in documents)
                {
                    // Get user who updated this document
                    UserDTO user = await _userService.GetUserByIdAsync(accessCokie,document.UpdatedByUserId);

                    // Ser user name who updated in document structure.
                    document.UpdatedByUserName = $"{user.FirstName} {user.LastName}";
                }

            }
            return documents;
        }
        public async Task<DocumentDTO> GetDocumentByIdAsync(string accessCokie,int id)
        {
            DocumentDTO document = null;
            try
            {
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Document/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    document = JsonConvert.DeserializeObject<DocumentDTO>(result);

                    // Get user who updated this document
                    UserDTO user = await _userService.GetUserByIdAsync(accessCokie, document.UpdatedByUserId);

                    // Ser user name who updated in document structure.
                    document.UpdatedByUserName = $"{user.FirstName} {user.LastName}";
                }
            }
            catch
            {
            }
            return document;
        }

        public Task<List<DocumentDTO>> GetDocumentByNameAsync(string docName)
        {
            throw new NotImplementedException();
        }
        public async Task<List<DocumentDTO>> GetDocumentByNameAsync(string accessCokie, string docName)
        {
            List<DocumentDTO> documents = null;
            try
            {
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync($"api/Document/{docName}/byname");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    documents = JsonConvert.DeserializeObject<List<DocumentDTO>>(result);

                    foreach (DocumentDTO document in documents)
                    {
                        // Get user who updated this document
                        UserDTO user = await _userService.GetUserByIdAsync(accessCokie, document.UpdatedByUserId);

                        // Ser user name who updated in document structure.
                        document.UpdatedByUserName = $"{user.FirstName} {user.LastName}";
                    }
                }
            }
            catch
            {
            }
            return documents;
        }
    }
}
