using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Extensions;
using backend_wonderservice.DATA.Models;
using Microsoft.EntityFrameworkCore;
using WonderService.Data.Services;

namespace backend_wonderservice.DATA.Repo
{
    public class ServicesUploadRepo : IServiceUpload
    {
        private readonly DataContext _context;

        public ServicesUploadRepo(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task Post(Services service)
        {
            await _context.Services.AddAsync(service);
            ;
        }

        public async Task<Services> Get(string id)
        {
            return await _context.Services.Include(r => r.Photo).Where(r => r.Id.Equals(Guid.Parse(id))).FirstOrDefaultAsync();
        }

        public async Task<List<Services>> GetAll()
        {
            return await _context.Services.Include(r => r.Photo).ToListAsync();
        }

        public Task Delete(Services model)
        {
            _context.Remove(model);
            return Task.CompletedTask;
        }

        public Task Update(Services model)
        {
            _context.Update(model);
            return Task.CompletedTask;
        }
    }

}
