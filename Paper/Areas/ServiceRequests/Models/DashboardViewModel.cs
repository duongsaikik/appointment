
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.ServiceRequests.Models
{
    public class DashboardViewModel
    {
        public List<ServiceRequestt> ServiceRequests { get; set; }
        public List<ServiceRequestt> AuditServiceRequests { get; set; }
        public Dictionary<string, int> ActiveServiceRequests { get; set; }
    }
}
