using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InitialSetupMVC.Services;
using System;
using System.Security.Claims;

namespace InitialSetupMVC.Controllers
{
    [Authorize(Roles = "Admin,User Distribution")]
    public class ApprovalController : Controller
    {
        private readonly ApprovalService _approvalService;
        private readonly RequestService _requestService;
        private readonly MedicineService _medService;

        public ApprovalController(ApprovalService approvalService, RequestService requestService, MedicineService medService)
        {
            _approvalService = approvalService;
            _requestService = requestService;
            _medService = medService;
        }

        public IActionResult Index()
        {
            var requests = _requestService.GetRequests();
            var logs = _approvalService.GetApprovalLogs();
            
            ViewBag.Logs = logs;
            ViewBag.Medicines = _medService.GetMedicines();
            return View(requests);
        }

        [HttpPost]
        public IActionResult Approve(long id, string remarks)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || string.IsNullOrEmpty(roleClaim))
            {
                TempData["ErrorMessage"] = "Authentication session error.";
                return RedirectToAction("Index");
            }

            try
            {
                _approvalService.Approve(id, userId, roleClaim, remarks);
                TempData["SuccessMessage"] = "Request has been approved.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(long id, string remarks)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || string.IsNullOrEmpty(roleClaim))
            {
                TempData["ErrorMessage"] = "Authentication session error.";
                return RedirectToAction("Index");
            }

            try
            {
                _approvalService.Reject(id, userId, roleClaim, remarks);
                TempData["SuccessMessage"] = "Request has been rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkDelivered(long id, string remarks)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || string.IsNullOrEmpty(roleClaim))
            {
                TempData["ErrorMessage"] = "Authentication session error.";
                return RedirectToAction("Index");
            }

            try
            {
                _approvalService.MarkDelivered(id, userId, roleClaim, remarks);
                TempData["SuccessMessage"] = "Request marked as Delivered.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
