using System.Collections.Generic;

namespace InitialSetupMVC.Models
{
    public class DashboardViewModel
    {
        public int TotalStock { get; set; }
        public int LowStockCount { get; set; }
        public int NormalStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        
        public int NearExpiredCount { get; set; }
        public int SafeExpiredCount { get; set; }
        
        public int TotalRequestsCount { get; set; }
        public int PendingRequestsCount { get; set; }
        
        public List<Request> RecentRequests { get; set; } = new();
        public List<Medicine> Medicines { get; set; } = new();
    }
}
