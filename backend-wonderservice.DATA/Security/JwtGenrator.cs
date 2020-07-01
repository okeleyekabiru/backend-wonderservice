using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using backend_wonderservice.DATA.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WonderService;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Plugins.JwtHandler
{
  public  class JwtGenrator:IJwtSecurity
    {
        private readonly IConfiguration _configuration;

        public JwtGenrator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public JwtModel CreateToken(User user,string userRole)
        {
           
            var claim = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(ClaimTypes.Role,userRole ??"User")


            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("jwtHandler").Value));
            var credentials =  new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claims:claim),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
           var token =  tokenHandler.CreateToken(tokenDescriptor);
          var returnToken = tokenHandler.WriteToken(token);

          return new JwtModel
          {
              ExpiryDate = token.ValidTo,
              Token = returnToken
          };
        }

        public JwtModel ReadToken(string token)
        {
           
            var handler = new JwtSecurityTokenHandler();
            var decodeToken = handler.ReadJwtToken(token);
        
                 return new JwtModel
                 {
                      ExpiryDate = decodeToken.ValidTo,
                   Token  = decodeToken.Claims?.First().Value
                 };
          
         
        }

        public
            JwtModel CreateTokenForEmail(User user)
        {
            var claim = new List<Claim>
            {

                new Claim(JwtRegisteredClaimNames.NameId, user.Id)


            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("jwtHandler").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: claim),
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var returnToken = tokenHandler.WriteToken(token);

            return new JwtModel
            {
                ExpiryDate = token.ValidTo,
                Token = returnToken
            };
        }
    }
}
