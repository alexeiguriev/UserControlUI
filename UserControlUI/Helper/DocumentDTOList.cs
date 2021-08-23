using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserControlUI.ModelsDTO
{
    static public class DocumentListConverter
    {
        static public List<DocumentDTO> SortByDate(List<DocumentDTO> inputList)
        {
            List<DocumentDTO> newList = new List<DocumentDTO>(inputList);
            // Sort list by date
            newList.Sort((x, y) => y.UploadedDate.CompareTo(x.UploadedDate));

            return newList;

        }
        static public List<List<DocumentDTO>> Convert(List<DocumentDTO> inputList)
        {
            List<List<DocumentDTO>> documents = new List<List<DocumentDTO>>();
            List<DocumentDTO> newList = new List<DocumentDTO>(inputList);
        
            if (newList != null)
            {
                while (newList.Count != 0)
                {
                    // Get fisrt element from list
                    DocumentDTO doc = newList[0];

                    List<DocumentDTO> actualList = newList.FindAll(d => d.Name == doc.Name);

                    // Sort list by date
                    newList = SortByDate(newList);

                    // Add all elements with the same name
                    documents.Add(actualList);

                    // Remove all actual elements
                    newList.RemoveAll(d => d.Name == doc.Name);

                }
            }
            return documents;
        }
    }
}
