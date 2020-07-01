using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.EntityFrameworkCore;
using WonderService.Data.Services;
using WonderService.Data.ViewModel;

namespace WonderService.Data.Repo
{
  public  class ServiceTypeRepo:IServiceType
    {
        private readonly DataContext _context;

        public ServiceTypeRepo(DataContext context)
        {
            _context = context;
        }
        public async Task<Guid> GetServiceId(string category)
        {
            category = category.Trim().ToLower();
                return await _context.ServicesTypes.Where(r => r.ServiceType.Equals(category)).Select(r => r.Id)
                  .FirstOrDefaultAsync();
            
        }

        public async Task Post(ServicesTypes services)
        {
           services.ServiceType = services.ServiceType.ToLower().Trim();
            await _context.AddAsync(services);
        }

        public  ServicesTypes Get(string id)
        {
           
                return _context.ServicesTypes.Find(Guid.Parse(id));
            
        }

        public ServicesTypes GetServicesType(string category)
        {
            return _context.ServicesTypes.FirstOrDefault(r => r.ServiceType.Equals(category));
        }

        public async Task<List<ServicesTypes>> GetAll()
        {
            return  await _context.ServicesTypes.ToListAsync();
        }

        public async Task<ReturnResult> SaveChanges()
        {
            ReturnResult result = new ReturnResult();
            if (await _context.SaveChangesAsync() > 0)
            {
                result.Succeeded = true;
                return result;
            }

            result.Error = "An error occured updating database";
            return result;
        }

        public Task Update(ServicesTypes services)
        {
            _context.Update(services);
            return  Task.CompletedTask;
        }

        public Task Delete(ServicesTypes services)
        {
            _context.Remove(services);
            return  Task.CompletedTask;
        }
    }
}
