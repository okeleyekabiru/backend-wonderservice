using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Models;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Abstration
{
    public interface IPhoto
    {
        Task Add(Photo photo);
        Task AddRange(List<Photo> photo);
        Task<Photo> Get(string id);
        Task Delete(Photo photo);
        Task<ReturnResult> SaveChanges();


    }
}
