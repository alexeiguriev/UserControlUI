using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.Models
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] ReachedByUserIds { get; set; }
    }
}
