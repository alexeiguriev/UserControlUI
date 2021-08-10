using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Helper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<(string str, IFormFile file), InputDocument>()
                .ForMember(d => d.UpdaterId, opt => opt.MapFrom(s => Newtonsoft.Json.JsonConvert.DeserializeObject<InputDocument>(s.str).UpdaterId))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => Newtonsoft.Json.JsonConvert.DeserializeObject<InputDocument>(s.str).Status))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.file.FileName))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.file.ContentType))
                .ForMember(d => d.Content, opt => opt.MapFrom(s => Converter.PdfToByteBuffConverter(s.file)));
        }
    }
}
