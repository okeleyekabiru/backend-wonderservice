using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace WonderService.Data.ViewModel
{
   public class OrderModel
    {
        public string Id { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "invalid email")]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string ServiceType { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentDateEnd { get; set; }
        public string LocalGovernment { get; set; }
        public string States { get; set; }
    }
}
