
using ASC.Business.Interface;
using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using AutoMapper;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Paper.Areas.ServiceRequests.Models;
using Paper.Controllers;
using Paper.Data;
using Paper.Hubs;
using Paper.Models;
using Paper.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.ServiceRequests.Controllers
{

    [Area("ServiceRequests")]
    public class ServiceRequestController : BaseController
    {
        private readonly IServiceRequestOperations _serviceRequestOperations;
        private readonly IServiceRequestMessageOperations _serviceRequestMessageOperations;
 
        private readonly IOptions<ApplicationSettings> _options;
      
        private readonly IMapper _mapper;
        private readonly IMasterDataCacheOperations _masterData;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServiceRequestController(IServiceRequestOperations operations,
            IServiceRequestMessageOperations messageOperations,

      
             IMapper   
             mapper, IMasterDataCacheOperations masterData,
            UserManager<ApplicationUser> userManager,
            IOptions<ApplicationSettings> options)
        {
            _serviceRequestOperations = operations;
            _serviceRequestMessageOperations = messageOperations;
          
            _mapper = mapper;
            _masterData = masterData;
            _userManager = userManager;
            _options = options;

        }
        [HttpGet]
        public async Task<IActionResult> ServiceRequest()
        {
            var masterData = await _masterData.GetMasterDataCacheAsync();
            ViewBag.ServiceTypes = masterData.Keys.ToList();

            return View(new NewServiceRequestViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> ServiceRequest(NewServiceRequestViewModel request)
        {
            var masterData = await _masterData.GetMasterDataCacheAsync();
            if (!ModelState.IsValid)
            {
               
                ViewBag.ServiceTypes = masterData.Keys.ToList();           
                return View(request);
            }
            // Map the view model to Azure model
            var serviceRequest = _mapper.Map<NewServiceRequestViewModel, ServiceRequestt>(request);
            // Set RowKey, PartitionKey, RequestedDate, Status properties
            serviceRequest.PartitionKey = HttpContext.User.GetCurrentUserDetails().Email;
            serviceRequest.RowKey = Guid.NewGuid().ToString();


            DateTime d = (DateTime)serviceRequest.RequestedDate;
            
            serviceRequest.RequestedDate = d.AddDays(1);

            var content = masterData.Keys.Where(p => p.RowKey == request.ServiceType.ToString()).ToList();
            serviceRequest.RequestedServices = content[0].Name.ToString();
            

            serviceRequest.Status = Status.New.ToString();
            await _serviceRequestOperations.CreateServiceRequestAsync(serviceRequest);
            return RedirectToAction("Dashboard", "Dashboard", new { Area = "ServiceRequests" });
        }
        [HttpGet]
        public async Task<IActionResult> ServiceRequestDetails(string id)
        {
            var serviceRequestDetails = await _serviceRequestOperations.GetServiceRequestByRowKey(id);
            // Access Check
            if (HttpContext.User.IsInRole(Roles.Doctor.ToString())
            && serviceRequestDetails.ServiceDoctor != HttpContext.User.
           GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }
            if (HttpContext.User.IsInRole(Roles.User.ToString())
            && serviceRequestDetails.PartitionKey != HttpContext.User.
           GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }
            var serviceRequestAuditDetails = await _serviceRequestOperations.GetServiceRequestAuditByPartitionKey(
            serviceRequestDetails.PartitionKey + "-" + id);
            // Select List Data
            var masterData = await _masterData.GetMasterDataCacheAsync();
            ViewBag.ServiceTypes = masterData.Keys.ToList();
            var content = serviceRequestDetails.RequestedServices;
            ViewBag.content = content;
            ViewBag.Status = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => v.ToString()).ToList();
            ViewBag.ServiceDoctor = await _userManager.GetUsersInRoleAsync(Roles.Doctor.ToString());
            return View(new ServiceRequestDetailViewModel
            {
                ServiceRequest = _mapper.Map<ServiceRequestt, UpdateServiceRequestViewModel>(serviceRequestDetails),
                ServiceRequestAudit = serviceRequestAuditDetails.OrderByDescending(p => p.Timestamp).ToList()
            });

        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceRequestDetails(UpdateServiceRequestViewModel serviceRequest)
        {
            var originalServiceRequest = await _serviceRequestOperations.GetServiceRequestByRowKey(serviceRequest.RowKey);
            originalServiceRequest.RequestedServices = serviceRequest.RequestedServices;
            // Update Status only if user role is either Admin or Engineer
            // Or Customer can update the status if it is only in Pending Customer Approval.
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()) ||
            HttpContext.User.IsInRole(Roles.Doctor.ToString()) ||
            (HttpContext.User.IsInRole(Roles.User.ToString()) && originalServiceRequest.Status == Status.PendingCustomerApproval.ToString()))
            {
                DateTime d = (DateTime)serviceRequest.RequestedDate;
                serviceRequest.RequestedDate = d.AddDays(1);
                originalServiceRequest.Status = serviceRequest.Status;
                originalServiceRequest.RequestedDate = serviceRequest.RequestedDate;
            }

            // Update Service Engineer field only if user role is Admin
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                originalServiceRequest.ServiceDoctor = serviceRequest.ServiceDoctor;
            }
            await _serviceRequestOperations.UpdateServiceRequestAsync(originalServiceRequest);
            return RedirectToAction("ServiceRequestDetails", "ServiceRequest",
            new { Area = "ServiceRequests", Id = serviceRequest.RowKey });
        }
        [HttpGet]
        public async Task<IActionResult> ServiceRequestMessages(string serviceRequestId)
        {
            return Json((await _serviceRequestMessageOperations.GetServiceRequestMessageAsync(serviceRequestId)).OrderByDescending(p => p.MessageDate));
        }
        [HttpPost]
        public async Task<IActionResult> CreateServiceRequestMessage(ServiceRequestMessage message)
        {
            // Message and Service Request Id (Service request Id is the partition key for a message)
            if (string.IsNullOrWhiteSpace(message.Message) || string.IsNullOrWhiteSpace(message.PartitionKey))
                return Json(false);
            // Get Service Request details
            var serviceRequesrDetails = await _serviceRequestOperations.GetServiceRequestByRowKey
           (message.PartitionKey);
            // Populate message details
            message.FromEmail = HttpContext.User.GetCurrentUserDetails().Email;
            message.FromDisplayName = HttpContext.User.GetCurrentUserDetails().Name;
            message.MessageDate = DateTime.UtcNow;
            message.RowKey = Guid.NewGuid().ToString();
            // Get Customer and Service Engineer names
            var customerName = (await _userManager.FindByEmailAsync(serviceRequesrDetails.
           PartitionKey)).UserName;
            var serviceEngineerName = string.Empty;
            if (!string.IsNullOrWhiteSpace(serviceRequesrDetails.ServiceDoctor))
            {
                serviceEngineerName = (await _userManager.FindByEmailAsync(serviceRequesrDetails.
               ServiceDoctor)).UserName;
            }
            var adminName = (await _userManager.FindByEmailAsync(_options.Value.AdminEmail)).
           UserName;
            // Save the message to Azure Storage
            await _serviceRequestMessageOperations.CreateServiceRequestMessageAsync(message);
            var users = new List<string> { customerName, adminName };
            if (!string.IsNullOrWhiteSpace(serviceEngineerName))
            {
                users.Add(serviceEngineerName);
            }
            // Broadcast the message to all clients asscoaited with Service Request


            // Return true
            return Json(true);
        }
    }

    }
