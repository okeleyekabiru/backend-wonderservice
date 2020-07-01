using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Extensions;
using backend_wonderservice.DATA.Models;

namespace backend_wonderservice.DATA.Abstration
{
    public interface IServiceUpload
    {
        Task<bool> SaveChanges();
        Task Post(Services service);
        Task<Services> Get(string id);
        Task<List<Services>> GetAll();
        Task Delete(Services model);
        Task Update(Services model);
    }
}
