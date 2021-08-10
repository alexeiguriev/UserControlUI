using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.ModelsDTO
{
    public class DocumentInputDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public DateTime UploadedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public FormFile file { get; set; }
    }
}
