using System;
using System.Collections.Generic;
using System.Text;


namespace WonderService.Data.ViewModel
{
  public  class ServiceReturnVm
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTime Entry { get; set; }
        public virtual ICollection<PhotoVm> Photos { get; set; }
    }
}
