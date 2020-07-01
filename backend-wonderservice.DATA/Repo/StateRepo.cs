using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.EntityFrameworkCore;

using WonderService.Data.Services;

namespace backend_wonderservice.DATA.Repo
{
   public class StateRepo:IStateRepo
    {
        private readonly DataContext _context;

        public StateRepo(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<States>> GetState()
        {
            return await _context.States.ToListAsync();
        }

        public async Task<short> GetId(string state)
        {
            var states = await _context.States.FirstOrDefaultAsync(r => r.Name.Equals(state));
            if (states == null) return default;
            return states.Id;
        }

        public async Task<IEnumerable<LocalGovernment>> GetLocalGovernment(short state)
        {
            return await _context.LocalGovernment.Where(r => r.StateId.Equals(state)).ToListAsync();
        }

        public async Task<short> GetLocalGovernmentId(string localGovernment)
        {
          var  localGovernments = await _context.LocalGovernment.FirstOrDefaultAsync(r => r.Name.Equals(localGovernment));
          if (localGovernments == null) return default;
          return localGovernments.Id;
        }
    }
}
