using System;
using System.Collections.Generic;
using System.Text;
using backend_wonderservice.DATA.Models;
using WonderService;


namespace Plugins.JwtHandler
{
   public interface IJwtSecurity
   {
       JwtModel CreateToken(User user, string userRole);


       JwtModel ReadToken(string token);

       JwtModel CreateTokenForEmail(User user);
    }
}
