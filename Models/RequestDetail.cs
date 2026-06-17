namespace InitialSetupMVC.Models
{
    public class RequestDetail
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public long MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
