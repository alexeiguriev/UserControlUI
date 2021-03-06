using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.ModelsDTO
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public DateTime UploadedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public string UpdatedByUserName { get; set; }
        public byte[] Content { get; set; }
    }
}
