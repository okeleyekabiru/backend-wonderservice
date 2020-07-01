using System;
using System.Collections.Generic;
using System.Text;

namespace Plugins.JwtHandler
{
   public class JwtModel
   {
      public string Token { get; set; }
      public DateTime ExpiryDate { get; set; }
      public  string Error { get; set; }
      public string Code { get; set; }
   }
}
