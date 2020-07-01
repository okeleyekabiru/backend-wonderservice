using System;
using System.Collections.Generic;
using System.Text;

namespace backend_wonderservice.DATA.Models
{
    public partial class Customer
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public Guid ServicesTypeId { get; set; }
        public DateTime AppointmentDateEnd { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public short StateId { get; set; }
        public short LocalGovernmentId { get; set; }

        public virtual LocalGovernment LocalGovernment { get; set; }
        public virtual ServicesTypes ServicesType { get; set; }
        public virtual States State { get; set; }
    }
}
