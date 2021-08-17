using AutoMapper;
using HelperCSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Threading.Tasks;
using UserControlUI.Models;
using UserControlUI.ModelsDTO;

namespace Auth.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        public UserController( HttpClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<UserDTO> users = null;
            try
            {
                users = await GetUsersAsync();
            }
            catch
            {

            }
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            List<UserDTO> users = null;
            try
            {
                users = await GetUsersAsync();
            }
            catch
            {
                BadRequest(null);
            }
            return Ok(users);

        }
        public async Task<IActionResult> Edit(UserDTO user)
        {
            // Create a value for delete role
            string[] roleNames = { "Delete Role" };

            // Get all user roles
            string[] allUserRoles = await GetRolesNamesListAsync();

            // Combine roles
            roleNames = roleNames.Concat(allUserRoles).ToArray();

            // Store in view data user roles
            ViewData["UserRoles"] = roleNames;

            // Store in view data user roles
            HttpContext.Session.SetInt32("UserToEditId", user.Id);

            return View(user);
        }
        public IActionResult Details(UserDTO user)
        {
            return View(user);
        }
        public IActionResult Delete(UserDTO user)
        {
            // Store Doc Id
            HttpContext.Session.SetInt32("UserToDeleteId", user.Id);

            // Run view
            return View(user);
        }
        public async Task<IActionResult> DeleteUser(UserDTO user)
        {
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                int id = (int)HttpContext.Session.GetInt32("UserToDeleteId");
                // HTTP POST
                HttpResponseMessage res = await _client.DeleteAsync("api/User/" + id);
                if (!res.IsSuccessStatusCode)
                {
                    // Store error message
                    TempData["UserIndexError"] = "Delete Error";
                    return RedirectToAction("Index", user);
                }
            }
            catch
            {
                // Store error message
                TempData["UserIndexError"] = "Delete Error";
                return RedirectToAction("Index", user);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddRole(UserDTO user)
        {
            user.Id = (int)HttpContext.Session.GetInt32("UserToEditId");

            // Get all role names
            List<RoleDTO> roles = await GetRolesAsync();

            List<string> newRoleNamesList = new List<string>(user.Roles);// { aString, bString }
            newRoleNamesList.Add(roles[0].Name);

            user.Roles = newRoleNamesList.ToArray();

            return RedirectToAction("Edit", user);
        }
        public async Task<IActionResult> Save(UserDTO user)
        {
            user.Id = (int)HttpContext.Session.GetInt32("UserToEditId");
            // Convert userDto to user Input
            UserInput newUser = _mapper.Map<UserInput>(user);

            List<RoleDTO> allRoles = await GetRolesAsync();
            List<int> roleIds = new List<int>();

            // Remove "DeleteRole" from array
            string numToRemove = "Delete Role";
            user.Roles = user.Roles.Where(val => val != numToRemove).ToArray();

            // User input data validation
            if (!UserValudated(user))
            {
                // Store error message
                TempData["UserEditError"] = "Error Input";
                return RedirectToAction("Edit", user);
            }

            // Confert roles names to user IDs
            foreach(string roleName in user.Roles)
            {
                roleIds.Add(allRoles.Find(ur => ur.Name == roleName).Id);
            }

            // Put role IDs in user roleIDs
            newUser.UserRolesId = roleIds.ToArray();

            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                // HTTP POST
                HttpResponseMessage res = await _client.PutAsJsonAsync("api/User/" + user.Id, newUser).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    // Store error message
                    TempData["UserEditError"] = "Error Input";
                    return RedirectToAction("Edit", user);
                }
            }
            catch
            {
                // Store error message
                TempData["UserEditError"] = "Error Input";
                return RedirectToAction("Edit", user);
            }

            return RedirectToAction("Index");
        }
        public async Task<string[]> GetRolesNamesListAsync()
        {
            List<RoleDTO> roles = await GetRolesAsync();
            List<string> roleNames = new List<string>();
            
            foreach(RoleDTO role in roles)
            {
                roleNames.Add(role.Name);
            }
            return roleNames.ToArray();
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
        public async Task<RoleDTO> GetRolesByIdAsync(int id)
        {
            RoleDTO role = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

                HttpResponseMessage res = await _client.GetAsync("api/Role/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    role = JsonConvert.DeserializeObject<RoleDTO>(result);
                }
            }
            catch
            {
            }
            return role;
        }
        public async Task<List<UserDTO>> GetUsersAsync()
        {
            List<UserDTO> users = null;
            try
            {
                //string accessCokie = ViewData["JWToken"] as string;
                string accessCokie = HttpContext.Session.GetString("JWToken");
                _client.DefaultRequestHeaders.Add("Cookie", accessCokie);

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
        [HttpGet]
        public  async Task<IActionResult> Get(UserInput loginInput)
        {
            UserDTO user = null;
            try
            {
                loginInput.Password = Crypt.EncodePasswordToBase64(loginInput.Password);

                // Setting content type.
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialization.
                HttpResponseMessage response = new HttpResponseMessage();

                // HTTP POST
                response = await _client.PostAsJsonAsync("api/login", loginInput).ConfigureAwait(false);

                // Verification
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.
                    string result = response.Content.ReadAsStringAsync().Result;

                    //Get cookie
                    string cookie = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.First();

                    //get user
                    user = JsonConvert.DeserializeObject<UserDTO>(result);

                    ViewData["JWToken"] = cookie;

                }
                else
                {
                    throw new AuthenticationException("Authentication failed");
                }
            }
            catch
            {
                throw new AuthenticationException("Authentication failed");
            }
            return Ok(user);
        }
        private bool UserValudated(UserDTO user)
        {
            bool retValue = true;
            if (user.Roles.Count() != user.Roles.Distinct().Count())
            {
                retValue &= false;
            }
            if (string.IsNullOrEmpty(user.FirstName))
            {
                retValue &= false;
            }
            if (string.IsNullOrEmpty(user.LastName))
            {
                retValue &= false;
            }
            if (string.IsNullOrEmpty(user.EmailAddress))
            {
                retValue &= false;
            }
            return retValue;
        }
    }
}
