using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.Models;

namespace UserControlUI.ModelsDTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int[] RoleIds { get; set; }
    }
}
