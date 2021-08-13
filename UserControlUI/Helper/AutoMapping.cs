using UserControlUI.ModelsDTO;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.Helper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap <IFormFile, InputDocument>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.FileName))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.ContentType))
                .ForMember(d => d.Content, opt => opt.MapFrom(s => Converter.PdfToByteBuffConverter(s)));
        }
    }
}
