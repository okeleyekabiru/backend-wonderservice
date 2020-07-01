using System;
using System.Collections.Generic;
using System.Text;

namespace WonderService.Data.ViewModel
{
   public class CustomerVm
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ServiceType { get; set; }
        public string UserId { get; set; }
    }
}
