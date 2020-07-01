using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Models;

namespace backend_wonderservice.DATA.Abstration
{
    public interface IStateRepo
    {
        Task<IEnumerable<States>> GetState();
        Task<short> GetId(string state);
        Task<IEnumerable<LocalGovernment>> GetLocalGovernment(short state);
        Task<short> GetLocalGovernmentId(string localGovernment);

    }
}
