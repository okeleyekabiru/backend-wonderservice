using System;
using System.Collections.Generic;

namespace backend_wonderservice.DATA.Models
{
    public class ServicesTypes
    {
        public ServicesTypes()
        {
            Customer = new HashSet<Customer>();
            Services = new HashSet<Services>();
        }

        public string ServiceType { get; set; }
        public Guid Id { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Services> Services { get; set; }
    }
}
