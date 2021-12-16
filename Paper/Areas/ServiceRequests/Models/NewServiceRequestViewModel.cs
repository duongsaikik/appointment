using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.ServiceRequests.Models
{
    public class NewServiceRequestViewModel
    {
        
      
        [Required]
        [Display(Name = "Service Type")]
        public string ServiceType { get; set; }
      
        [Display(Name = "Requested Services")]
        public string RequestedServices { get; set; }
        [Required]
        [Display(Name = "Requested Date")]
        public DateTime? RequestedDate { get; set; }
    }
}
