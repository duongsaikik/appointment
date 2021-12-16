using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.ServiceRequests.Models
{
    public class UpdateServiceRequestViewModel:NewServiceRequestViewModel
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        [Required]
        [Display(Name = "Service Doctor")]
        public string ServiceDoctor { get; set; }
       
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
