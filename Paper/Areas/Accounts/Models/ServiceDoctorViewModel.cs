using Paper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.Accounts.Models
{
    public class ServiceDoctorViewModel
    {
        public List<ApplicationUser> ServiceDoctors { get; set; }
        public ServiceDoctorRegistrationViewModel Registration { get; set; }
    }
}
