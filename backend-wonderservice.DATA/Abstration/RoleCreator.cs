using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Identity;

namespace backend_wonderservice.DATA.Abstration
{
    public class RoleCreator
    {





        public async Task Create(RoleManager<IdentityRole> _roleManager)
        {
            bool x = await _roleManager.RoleExistsAsync("Admin");
            if (!x)
            {
                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);

                //Here we create a Admin super user who will maintain the website                   



            }

            // creating Creating Manager role     
            x = await _roleManager.RoleExistsAsync("User");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "User";
                await _roleManager.CreateAsync(role);
            }

        }
    }

}
