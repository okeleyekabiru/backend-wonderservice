using System;
using System.Collections.Generic;

namespace backend_wonderservice.DATA.Models
{
    public partial class Services
    {
        public Services()
        {
            Photo = new HashSet<Photo>();
        }

        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public DateTime Entry { get; set; }

        public virtual ServicesTypes ServiceType { get; set; }
        public virtual ICollection<Photo> Photo { get; set; }
    }
}