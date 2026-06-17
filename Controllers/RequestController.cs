using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InitialSetupMVC.Models;
using InitialSetupMVC.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace InitialSetupMVC.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly RequestService _requestService;
        private readonly MedicineService _medService;

        public RequestController(RequestService requestService, MedicineService medService)
        {
            _requestService = requestService;
            _medService = medService;
        }

        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<Request> list;

            if (User.IsInRole("External User") && long.TryParse(userIdClaim, out long userId))
            {
                list = _requestService.GetRequestsForUser(userId);
            }
            else
            {
                list = _requestService.GetRequests();
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!User.IsInRole("External User"))
            {
                TempData["ErrorMessage"] = "Only External Users are permitted to create new request orders. Admins and Distribution users may view records and queues.";
                return RedirectToAction("Index");
            }

            var medicinesInStock = _medService.GetMedicines();
            return View(medicinesInStock);
        }

        [HttpPost]
        public IActionResult SubmitRequest([FromBody] RequestSubmissionDto dto)
        {
            if (!User.IsInRole("External User"))
            {
                return Json(new { success = false, message = "Only External Users are authorized to submit requests." });
            }

            if (dto == null || dto.Items == null || !dto.Items.Any())
            {
                return Json(new { success = false, message = "Cart items list is empty." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Json(new { success = false, message = "User ID claim not found in active session." });
            }

            var userFullName = User.Identity?.Name ?? "External User";

            try
            {
                var serviceItems = dto.Items.Select(i => new RequestItemDto
                {
                    MedicineId = i.MedicineId,
                    Qty = i.Qty
                }).ToList();

                _requestService.SubmitRequest(userId, userFullName, serviceItems, dto.Remarks);
                return Json(new { success = true, message = "Your drug request has been submitted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class RequestSubmissionDto
    {
        public string Remarks { get; set; } = string.Empty;
        public List<RequestItemDto> Items { get; set; } = new();
    }
}
