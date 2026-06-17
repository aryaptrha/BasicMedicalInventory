using InitialSetupMVC.Models;
using InitialSetupMVC.Repositories;
using System.Collections.Generic;

namespace InitialSetupMVC.Services
{
    public class MedicineService
    {
        private readonly MedicineRepository _medRepo;

        public MedicineService(MedicineRepository medRepo)
        {
            _medRepo = medRepo;
        }

        public List<Medicine> GetMedicines()
        {
            return _medRepo.GetAll();
        }

        public Medicine? GetMedicineById(long id)
        {
            return _medRepo.GetById(id);
        }

        public void AddMedicine(Medicine med)
        {
            _medRepo.Create(med);
        }

        public void UpdateMedicine(Medicine med)
        {
            _medRepo.Update(med);
        }

        public void DeleteMedicine(long id)
        {
            _medRepo.Delete(id);
        }
    }
}
