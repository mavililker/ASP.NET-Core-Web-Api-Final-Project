using EventProjectFinal.DTOs;
using EventProjectFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EventProjectFinal.Controllers
{
    
    [Authorize(Roles = "Admin, Company")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class CompanyController : ControllerBase
    {
        private readonly EVENTS3Context _context;
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(EVENTS3Context context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public IActionResult GetEvents()
        {
            try
            {
                List<EventDTO> data = (from e in _context.Events
                                       join o in _context.Categories on e.CategoryId equals o.CategoryId
                                       join c in _context.Cities on e.CityId equals c.CityId
                                       select new EventDTO()
                                       {
                                           EventId = e.EventId,
                                           EventName = e.EventName,
                                           CategoryId = e.CategoryId,
                                           CityId = e.CityId,
                                           CategoryName = o.CategoryName,
                                           CityName = c.CityName,
                                           IsCanceled = e.IsCanceled,

                                       }).Distinct().ToList();
                var username = _context.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                _logger.LogInformation("Get Events used by {} in {}", username, DateTime.Now);
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }

        }


    }
}
