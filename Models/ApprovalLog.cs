using System;

namespace InitialSetupMVC.Models
{
    public class ApprovalLog
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public long ActionBy { get; set; }
        public string ActionByName { get; set; } = string.Empty;
        public string ActionByRole { get; set; } = string.Empty;
        public string RequestNumber { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty; // e.g. "Submit", "Admin Approved", "Distribution Approved", "Delivered"
        public string Remarks { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}
