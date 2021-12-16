using ASC.Business.Interface;
using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paper.Areas.ServiceRequests.Models;
using Paper.Controllers;
using Paper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.ServiceRequests.Controllers
{

    [Area("ServiceRequests")]
    public class DashboardController : BaseController
    {
        private readonly IServiceRequestOperations _serviceRequestOperations;
        private readonly IMasterDataCacheOperations _masterData;
        public DashboardController(IServiceRequestOperations operations, IMasterDataCacheOperations
masterData)
        {
            _serviceRequestOperations = operations;
            _masterData = masterData;
        }
        public async Task<IActionResult> Dashboard()
        {
           // List of Status which were to be queried.
            var status = new List<string>
                     {
                         Status.New.ToString(),
                         Status.InProgress.ToString(),
                         Status.Initiated.ToString(),
                         Status.RequestForInformation.ToString()
                     };
            List<ServiceRequestt> serviceRequests = new List<ServiceRequestt>();
            List<ServiceRequestt> auditServiceRequests = new List<ServiceRequestt>();
            Dictionary<string, int> activeServiceRequests = new Dictionary<string, int>();
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                serviceRequests = await _serviceRequestOperations.
               GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddDays(-7),
                status);
                auditServiceRequests = await _serviceRequestOperations.GetServiceRequestsFormAudit();
                var serviceEngineerServiceRequests = await _serviceRequestOperations.GetActiveServiceRequests(new List<string>
                     {
                     Status.InProgress.ToString(),
                     Status.Initiated.ToString(),
                     });
                if (serviceEngineerServiceRequests.Any())
                {
                    activeServiceRequests = serviceEngineerServiceRequests
                    .GroupBy(x => x.ServiceDoctor)
                    .ToDictionary(p => p.Key, p => p.Count());
                }
            }
            else if (HttpContext.User.IsInRole(Roles.Doctor.ToString()))
            {
                serviceRequests = await _serviceRequestOperations.
               GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddDays(-7),
                status,
                serviceEngineerEmail: HttpContext.User.GetCurrentUserDetails().Email);
                auditServiceRequests = await _serviceRequestOperations.GetServiceRequestsFormAudit(HttpContext.User.GetCurrentUserDetails().Email);
            }
            else
            {
                serviceRequests = await _serviceRequestOperations.
               GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddYears(-1),
                email: HttpContext.User.GetCurrentUserDetails().Email);
            }
            var orderedAudit = auditServiceRequests.OrderByDescending(p => p.Timestamp).ToList();
            return View(new DashboardViewModel
            {
                ServiceRequests = serviceRequests.OrderByDescending(p => p.RequestedDate).ToList(),
                AuditServiceRequests = orderedAudit,
                ActiveServiceRequests = activeServiceRequests
            });
         
        }
    }
}
