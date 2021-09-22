using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserControlUI.ModelsDTO;

namespace UserControlUI.Services
{
    public interface IDocumentService
    {
        Task<List<DocumentDTO>> GetAllDocuments(string accessCokie);
        Task<DocumentDTO> GetDocumentByIdAsync(string accessCokie, int id);
        Task<List<DocumentDTO>> GetDocumentByNameAsync(string accessCokie, string docName);
    }
}
