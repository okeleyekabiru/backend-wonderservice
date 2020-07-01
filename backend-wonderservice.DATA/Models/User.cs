using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace backend_wonderservice.DATA.Models
{
    public partial class User : IdentityUser
    {



        public string FirstName { get; set; }
        public string LastName { get; set; }


        public virtual ICollection<Photo> Photo { get; set; }
    }
}