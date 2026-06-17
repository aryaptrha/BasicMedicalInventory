using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InitialSetupMVC.Models;
using InitialSetupMVC.Services;
using System;

namespace InitialSetupMVC.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly MedicineService _medService;

        public MedicineController(MedicineService medService)
        {
            _medService = medService;
        }

        public IActionResult Index()
        {
            var list = _medService.GetMedicines();
            return View(list);
        }

        [HttpPost]
        public IActionResult Create(string medicineName, int stockQty, decimal price, DateTime expiredDate)
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized. Only Admins can add medicines.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(medicineName))
            {
                TempData["ErrorMessage"] = "Medicine name is required.";
                return RedirectToAction("Index");
            }

            if (stockQty < 0)
            {
                TempData["ErrorMessage"] = "Stock quantity cannot be negative.";
                return RedirectToAction("Index");
            }

            if (price < 0)
            {
                TempData["ErrorMessage"] = "Price cannot be negative.";
                return RedirectToAction("Index");
            }

            var medicine = new Medicine
            {
                MedicineName = medicineName.Trim(),
                StockQty = stockQty,
                Price = price,
                ExpiredDate = expiredDate
            };

            try
            {
                _medService.AddMedicine(medicine);
                TempData["SuccessMessage"] = $"Medicine '{medicine.MedicineName}' has been added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving medicine: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(long id, string medicineName, int stockQty, decimal price, DateTime expiredDate)
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized. Only Admins can edit medicines.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(medicineName))
            {
                TempData["ErrorMessage"] = "Medicine name is required.";
                return RedirectToAction("Index");
            }

            if (stockQty < 0)
            {
                TempData["ErrorMessage"] = "Stock quantity cannot be negative.";
                return RedirectToAction("Index");
            }

            if (price < 0)
            {
                TempData["ErrorMessage"] = "Price cannot be negative.";
                return RedirectToAction("Index");
            }

            var medicine = _medService.GetMedicineById(id);
            if (medicine == null)
            {
                TempData["ErrorMessage"] = "Medicine not found.";
                return RedirectToAction("Index");
            }

            medicine.MedicineName = medicineName.Trim();
            medicine.StockQty = stockQty;
            medicine.Price = price;
            medicine.ExpiredDate = expiredDate;

            try
            {
                _medService.UpdateMedicine(medicine);
                TempData["SuccessMessage"] = $"Medicine '{medicine.MedicineName}' has been updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating medicine: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Unauthorized. Only Admins can delete medicines.";
                return RedirectToAction("Index");
            }

            var medicine = _medService.GetMedicineById(id);
            if (medicine == null)
            {
                TempData["ErrorMessage"] = "Medicine not found.";
                return RedirectToAction("Index");
            }

            try
            {
                _medService.DeleteMedicine(id);
                TempData["SuccessMessage"] = $"Medicine '{medicine.MedicineName}' has been deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting medicine: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
