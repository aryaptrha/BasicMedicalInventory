using InitialSetupMVC.Models;
using InitialSetupMVC.Repositories;
using System;

namespace InitialSetupMVC.Services
{
    public class ApprovalService
    {
        private readonly RequestRepository _requestRepo;
        private readonly MedicineRepository _medRepo;

        public ApprovalService(RequestRepository requestRepo, MedicineRepository medRepo)
        {
            _requestRepo = requestRepo;
            _medRepo = medRepo;
        }

        public List<ApprovalLog> GetApprovalLogs()
        {
            return _requestRepo.GetAllLogs();
        }

        public void Approve(long requestId, long userId, string roleName, string remarks)
        {
            var req = _requestRepo.GetById(requestId);
            if (req == null)
            {
                throw new ArgumentException("Request not found.");
            }

            var log = new ApprovalLog
            {
                RequestId = requestId,
                ActionBy = userId,
                Remarks = string.IsNullOrWhiteSpace(remarks) ? $"Request approved by {roleName}." : remarks.Trim()
            };

            if (roleName == "Admin")
            {
                if (req.Status != "Submitted")
                {
                    throw new InvalidOperationException("Request is not in a status that can be approved by Admin.");
                }

                // Double check stock availability
                foreach (var detail in req.Details)
                {
                    var med = _medRepo.GetById(detail.MedicineId);
                    if (med == null)
                    {
                        throw new InvalidOperationException($"Medicine '{detail.MedicineName}' does not exist in inventory.");
                    }
                    if (med.StockQty < detail.Qty)
                    {
                        throw new InvalidOperationException($"Insufficient stock for '{med.MedicineName}'. Required: {detail.Qty}, Available: {med.StockQty}.");
                    }
                }

                log.ActionType = "Admin Approved";
                // This updates the status and deducts the inventory atomically in a transaction when Admin approves
                _requestRepo.UpdateStatusAndDeductStock(requestId, "Approved By Admin", null, req.Details, log);
            }
            else if (roleName == "User Distribution")
            {
                if (req.Status != "Approved By Admin")
                {
                    throw new InvalidOperationException("Request must be approved by Admin before distribution approval.");
                }

                log.ActionType = "Distribution Approved";
                // Since stock was already deducted by Admin, just update the status to Fully Approved
                _requestRepo.UpdateStatus(requestId, "Fully Approved", null, DateTime.UtcNow, null, log);
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorized role for approval action.");
            }
        }

        public void Reject(long requestId, long userId, string roleName, string remarks)
        {
            if (string.IsNullOrWhiteSpace(remarks))
            {
                throw new ArgumentException("Remarks are mandatory for rejections.");
            }

            var req = _requestRepo.GetById(requestId);
            if (req == null)
            {
                throw new ArgumentException("Request not found.");
            }

            var log = new ApprovalLog
            {
                RequestId = requestId,
                ActionBy = userId,
                Remarks = remarks.Trim()
            };

            string newStatus;

            if (roleName == "Admin")
            {
                if (req.Status != "Submitted")
                {
                    throw new InvalidOperationException("Request cannot be rejected at this stage.");
                }
                log.ActionType = "Admin Rejected";
                newStatus = "Rejected By Admin";
            }
            else if (roleName == "User Distribution")
            {
                if (req.Status != "Approved By Admin")
                {
                    throw new InvalidOperationException("Request cannot be rejected at this stage.");
                }
                log.ActionType = "Distribution Rejected";
                newStatus = "Rejected By Distribution";
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorized role for rejection action.");
            }

            _requestRepo.UpdateStatus(requestId, newStatus, null, null, null, log);
        }

        public void MarkDelivered(long requestId, long userId, string roleName, string remarks)
        {
            if (roleName != "User Distribution")
            {
                throw new UnauthorizedAccessException("Only User Distribution can confirm drug deliveries.");
            }

            var req = _requestRepo.GetById(requestId);
            if (req == null)
            {
                throw new ArgumentException("Request not found.");
            }

            if (req.Status != "Fully Approved")
            {
                throw new InvalidOperationException("Request must be fully approved before delivery confirm.");
            }

            var log = new ApprovalLog
            {
                RequestId = requestId,
                ActionBy = userId,
                ActionType = "Delivered",
                Remarks = string.IsNullOrWhiteSpace(remarks) ? "Medicines delivered to requestor." : remarks.Trim()
            };

            _requestRepo.UpdateStatus(requestId, "Delivered", null, null, DateTime.UtcNow, log);
        }
    }
}
