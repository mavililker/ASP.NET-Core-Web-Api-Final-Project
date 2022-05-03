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
    [Authorize(Roles = "Admin, Organizator")]

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrganizatorController : ControllerBase
    {
        private readonly EVENTS3Context _context;
        private readonly ILogger<CompanyController> _logger;

        public OrganizatorController(EVENTS3Context context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                return Ok(_context.Categories);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }

        }
        [HttpGet]
        public IActionResult GetCities()
        {
            try
            {
                return Ok(_context.Cities);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }

        }
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


        [HttpPost]
        public IActionResult AddEvent([FromBody] Event eventt)
        {
            try
            {
                eventt.OrganizatorID = _context.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var query = (from e in _context.Events where e.EventName == eventt.EventName select e);
                Event original = query.SingleOrDefault();
                if (original == null)
                {
                    _context.Events.Add(eventt);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return StatusCode(400, "Already exists.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }

        }

       

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateEvent(int id, Event eventt)
        {
            try
            {
                var query = (from e in _context.Events where e.EventId == id select e);
                Event original = query.FirstOrDefault();
                var userid = _context.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userid == original.OrganizatorID)
                {       
                    if ((original.Date - DateTime.Now).TotalDays >= 5)
                    {

                        original.IsCanceled = eventt.IsCanceled;
                        original.Capacity = eventt.Capacity;
                        original.Address = eventt.Address;

                        _context.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(400, "Less than 5 days left");
                    }
                }
                else
                {
                    return StatusCode(400, "Please update 'your' event.");
                }
            }
            catch (Exception)
            {

                return StatusCode(400, "Please select event that exists.");
            }

        }




    }
}
