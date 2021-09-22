using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        public UserService(HttpClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }
        public async Task<List<UserDTO>> GetUsersAsync(string accessCokieString)
        {
            List<UserDTO> users = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                _client.DefaultRequestHeaders.Add("Cookie", accessCokieString);

                HttpResponseMessage res = await _client.GetAsync("api/User");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<UserDTO>>(result);
                }
            }
            catch
            {
            }
            return users;
        }
        public async Task<UserDTO> GetUserByIdAsync(string accessCokie, int id)
        {
            UserDTO user = null;
            try
            {
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
    }
}
