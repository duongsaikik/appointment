using ASC.Business.Interface;

using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Models.Queries;
using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business
{
   public class ServiceRequestOperations : IServiceRequestOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceRequestOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateServiceRequestAsync(ServiceRequestt request)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<ServiceRequestt>().AddAsync(request);
                _unitOfWork.CommitTransaction();
            }
        }
        public async Task<ServiceRequestt> UpdateServiceRequestAsync(ServiceRequestt request)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<ServiceRequestt>().UpdateAsync(request);
                _unitOfWork.CommitTransaction();
                return request;
            }
        }
        public async Task<ServiceRequestt> UpdateServiceRequestStatusAsync(string rowKey,
       string partitionKey, string status)
        {
            using (_unitOfWork)
            {
                var serviceRequest = await _unitOfWork.Responitory<ServiceRequestt>().FindAsync(partitionKey, rowKey);
                if (serviceRequest == null)
                    throw new NullReferenceException();
                serviceRequest.Status = status;
                await _unitOfWork.Responitory<ServiceRequestt>().UpdateAsync(serviceRequest);
                _unitOfWork.CommitTransaction();
                return serviceRequest;
            }
        }
        public async Task<List<ServiceRequestt>> GetServiceRequestsByRequestedDateAndStatus
                (DateTime? requestedDate,
                 List<string> status = null,
                 string email = "",
                 string serviceEngineerEmail = "")
                        {
            var query = Queries.GetDashboardQuery(requestedDate, status, email,
           serviceEngineerEmail);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequestt>().
           FindAllByQuery(query);
            return serviceRequests.ToList();
        }

        public async Task<List<ServiceRequestt>> GetServiceRequestsFormAudit(string serviceDoctorEmail = "")
        {
            var query = Queries.GetDashboardAuditQuery(serviceDoctorEmail);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequestt>().FindAllInAuditByQuery(query);
            return serviceRequests.ToList();
        }

        public async Task<List<ServiceRequestt>> GetActiveServiceRequests(List<string> status)
        {
            var query = Queries.GetDashboardServiceEngineersQuery(status);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequestt>().FindAllByQuery(query);
            return serviceRequests.ToList();
        }

        public async Task<ServiceRequestt> GetServiceRequestByRowKey(string id)
        {
            var query = Queries.GetServiceRequestDetailsQuery(id);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequestt>().FindAllByQuery(query);
            return serviceRequests.FirstOrDefault();
        }

        public async Task<List<ServiceRequestt>> GetServiceRequestAuditByPartitionKey(string id)
        {
            var query = Queries.GetServiceRequestAuditDetailsQuery(id);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequestt>().FindAllInAuditByQuery(query);
            return serviceRequests.ToList();
        }
    }
}
