using InitialSetupMVC.Models;
using InitialSetupMVC.Repositories;
using System;
using System.Linq;

namespace InitialSetupMVC.Services
{
    public class DashboardService
    {
        private readonly MedicineRepository _medRepo;
        private readonly RequestRepository _requestRepo;

        public DashboardService(MedicineRepository medRepo, RequestRepository requestRepo)
        {
            _medRepo = medRepo;
            _requestRepo = requestRepo;
        }

        public DashboardViewModel GetDashboardData()
        {
            var today = DateTime.Today;
            var hPlus7 = today.AddDays(7);

            var medicines = _medRepo.GetAll();
            var requests = _requestRepo.GetAll();

            return new DashboardViewModel
            {
                TotalStock = medicines.Sum(m => m.StockQty),
                LowStockCount = medicines.Count(m => m.StockQty < 10 && m.StockQty > 0),
                NormalStockCount = medicines.Count(m => m.StockQty >= 10),
                OutOfStockCount = medicines.Count(m => m.StockQty == 0),
                
                NearExpiredCount = medicines.Count(m => m.ExpiredDate <= hPlus7),
                SafeExpiredCount = medicines.Count(m => m.ExpiredDate > hPlus7),
                
                TotalRequestsCount = requests.Count,
                PendingRequestsCount = requests.Count(r => r.Status == "Submitted" || r.Status == "Approved By Admin"),
                
                RecentRequests = requests.OrderByDescending(r => r.RequestDate).Take(5).ToList(),
                Medicines = medicines
            };
        }
    }
}
