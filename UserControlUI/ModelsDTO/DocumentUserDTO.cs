using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.ModelsDTO
{
    public class DocumentUserDTO
    {
        public List<DocumentDTO> Document { get; set; }
        public UserDTO User { get; set; }
    }
}
