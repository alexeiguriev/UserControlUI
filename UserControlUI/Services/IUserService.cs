using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsersAsync(string accessCokieString);
        Task<UserDTO> GetUserByIdAsync(string accessCokieString,int id);
    }
}
