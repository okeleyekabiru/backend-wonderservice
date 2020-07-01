using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Models;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Abstration
{
    public interface IServiceType
    {
        Task<Guid> GetServiceId(string category);
        Task Post(ServicesTypes services);
        ServicesTypes Get(string id);
        ServicesTypes GetServicesType(string category);
        Task<List<ServicesTypes>> GetAll();
        Task<ReturnResult> SaveChanges();
        Task Update(ServicesTypes services);
        Task Delete(ServicesTypes services);
    }
}
