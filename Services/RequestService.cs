using InitialSetupMVC.Models;
using InitialSetupMVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InitialSetupMVC.Services
{
    public class RequestService
    {
        private readonly RequestRepository _requestRepo;
        private readonly MedicineRepository _medRepo;

        public RequestService(RequestRepository requestRepo, MedicineRepository medRepo)
        {
            _requestRepo = requestRepo;
            _medRepo = medRepo;
        }

        public List<Request> GetRequests()
        {
            return _requestRepo.GetAll();
        }

        public List<Request> GetRequestsForUser(long userId)
        {
            return _requestRepo.GetAll().Where(r => r.UserId == userId).ToList();
        }

        public Request? GetRequestById(long id)
        {
            return _requestRepo.GetById(id);
        }

        public void SubmitRequest(long userId, string userFullName, List<RequestItemDto> items, string remarks)
        {
            if (items == null || !items.Any())
            {
                throw new ArgumentException("Request cart cannot be empty.");
            }

            var details = new List<RequestDetail>();
            
            foreach (var item in items)
            {
                if (item.Qty <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero.");
                }

                var med = _medRepo.GetById(item.MedicineId);
                if (med == null)
                {
                    throw new ArgumentException("One of the selected medicines could not be found.");
                }

                if (item.Qty > med.StockQty)
                {
                    throw new InvalidOperationException($"Requested quantity for '{med.MedicineName}' exceeds available stock ({med.StockQty} available).");
                }

                details.Add(new RequestDetail
                {
                    MedicineId = med.Id,
                    MedicineName = med.MedicineName,
                    Qty = item.Qty,
                    Price = med.Price
                });
            }

            // Generate Request Number (e.g. REQ-YYYYMMDD-[incremental])
            var dateStr = DateTime.Today.ToString("yyyyMMdd");
            var allRequestsToday = _requestRepo.GetAll().Where(r => r.RequestDate.Date == DateTime.Today).ToList();
            var countToday = allRequestsToday.Count + 1;
            var reqNumber = $"REQ-{dateStr}-{countToday:D3}";

            var req = new Request
            {
                RequestNumber = reqNumber,
                UserId = userId,
                UserFullName = userFullName,
                Status = "Submitted",
                RequestDate = DateTime.Now,
                Details = details
            };

            var initialLog = new ApprovalLog
            {
                ActionBy = userId,
                ActionType = "Submit",
                Remarks = string.IsNullOrWhiteSpace(remarks) ? "Submitted new medicine request." : remarks.Trim()
            };

            _requestRepo.Create(req, initialLog);
        }
    }

    public class RequestItemDto
    {
        public long MedicineId { get; set; }
        public int Qty { get; set; }
    }
}
