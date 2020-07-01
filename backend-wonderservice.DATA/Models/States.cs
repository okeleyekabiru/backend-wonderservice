using System.Collections.Generic;

namespace backend_wonderservice.DATA.Models
{
    public partial class States
    {
        public States()
        {
            Customer = new HashSet<Customer>();
            LocalGovernment = new HashSet<LocalGovernment>();
        }

        public short Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<LocalGovernment> LocalGovernment { get; set; }
    }
}
