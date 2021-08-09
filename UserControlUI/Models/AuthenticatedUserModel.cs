using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Models
{
    public class AuthenticatedUserModel
    {
        public string Access_Token { get; set; }
        public UserDTO user { get; set; }
    }
}
