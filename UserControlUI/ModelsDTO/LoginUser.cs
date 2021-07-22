using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.Models
{
    public class LoginUser
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
