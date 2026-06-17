using System;
using System.Collections.Generic;

namespace InitialSetupMVC.Models
{
    public class Request
    {
        public long Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved By Admin, Rejected By Admin, Approved By Distribution, Rejected By Distribution, Fully Approved, Delivered
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? AdminApprovedAt { get; set; }
        public DateTime? DistributionApprovedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public List<RequestDetail> Details { get; set; } = new();
        public List<ApprovalLog> Logs { get; set; } = new();
    }
}
