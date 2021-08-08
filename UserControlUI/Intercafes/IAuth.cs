using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Intercafes
{
    public interface IAuth
    {
        void Clear();
        string Cookie { get; set; }
        UserDTO User { get; set; }

    }
}
