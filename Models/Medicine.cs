using System;

namespace InitialSetupMVC.Models
{
    public class Medicine
    {
        public long Id { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int StockQty { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
