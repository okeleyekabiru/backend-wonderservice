using System.Collections.Generic;

namespace backend_wonderservice.DATA.Models
{
    public partial class LocalGovernment
    {
        public LocalGovernment()
        {
            Customer = new HashSet<Customer>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public short StateId { get; set; }

        public virtual States State { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
    }
}