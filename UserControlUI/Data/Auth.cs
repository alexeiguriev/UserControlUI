using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.Intercafes;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Data
{
    static public class Auth
    {
        static public string Cookie { get; set; }
        static public UserDTO User { get; set; }
        static public void Clear()
        {
            Cookie = null;
            User = null;
        }
    }
}
