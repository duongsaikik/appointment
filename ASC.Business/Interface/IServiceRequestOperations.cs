using ASC.Models.BaseTypes;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business.Interface
{
   public interface IServiceRequestOperations
    {
        Task CreateServiceRequestAsync(ServiceRequestt request);
        Task<ServiceRequestt> UpdateServiceRequestAsync(ServiceRequestt request);
        Task<ServiceRequestt> UpdateServiceRequestStatusAsync(string rowKey, string partitionKey, string status);
        Task<List<ServiceRequestt>> GetServiceRequestsByRequestedDateAndStatus
            (DateTime? requestedDate,
              List<string> status = null,
              string email = "",
              string serviceEngineerEmail = "");
        Task<List<ServiceRequestt>> GetServiceRequestsFormAudit(string serviceDoctorEmail = "");
        Task<List<ServiceRequestt>> GetActiveServiceRequests(List<string> status);
        Task<ServiceRequestt> GetServiceRequestByRowKey(string id);
        Task<List<ServiceRequestt>> GetServiceRequestAuditByPartitionKey(string id);

    }

}
