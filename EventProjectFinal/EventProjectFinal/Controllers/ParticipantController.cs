using EventProjectFinal.DTOs;
using EventProjectFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EventProjectFinal.Controllers
{
    [Authorize(Roles = "Admin, Organizator, Participant")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly EVENTS3Context _context;

        public ParticipantController(EVENTS3Context context)
        {
            _context = context;
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

        [HttpGet]
        public IActionResult GetEvents()
        {
            try
            {
                return Ok(_context.Events);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetEventsByCategory(int id, Event eventt)
        {
            try
            {
                List<EventDTO> data = (from e in _context.Events
                                       join o in _context.Categories on e.CategoryId equals o.CategoryId
                                       join c in _context.Cities on e.CityId equals c.CityId
                                       where e.CategoryId == id
                                       select new EventDTO()
                                       {
                                           EventName = e.EventName,
                                           CategoryName = o.CategoryName,
                                           CityName = c.CityName,
                                           CategoryId = e.CategoryId,
                                           CityId = c.CityId,
                                       }).ToList();

                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }


        }
        [HttpGet("{id}")]
        public IActionResult GetEventsByCity(int id, Event eventt)
        {
            try
            {
                List<EventDTO> data = (from e in _context.Events
                                       join o in _context.Categories on e.CategoryId equals o.CategoryId
                                       join c in _context.Cities on e.CityId equals c.CityId
                                       where e.CityId == id
                                       select new EventDTO()
                                       {
                                           EventName = e.EventName,
                                           CategoryName = o.CategoryName,
                                           CityName = c.CityName,
                                           CategoryId = e.CategoryId,
                                           CityId = c.CityId,
                                       }).ToList();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }

        }
        [HttpPost]
        public IActionResult AddEventUser([FromBody] EventUser eventUser)
        {
            try
            {
                var userid = _context.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); 
                var query = (from e in _context.EventUsers where e.UserId == userid && e.EventId == eventUser.EventId && e.IsAttend == "Yes" select e);
                EventUser original = query.FirstOrDefault();
                if (original == null)
                {
                    eventUser.UserId = _context.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.EventUsers.Add(eventUser);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return StatusCode(400, "You are already in the attandance list.");
                }
            }
            catch (Exception)
            {

                return StatusCode(500, "Please select from events that exist.");
            }

        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateEvent(int id, EventUser eventUser)
        {
            try
            {
                var userid = _context.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                
                    var query = (from e in _context.Events
                                 join u in _context.EventUsers on e.EventId equals u.EventId
                                 where e.EventId == id
                                 select e);

                    var query2 = (from e in _context.Events
                                  join u in _context.EventUsers on e.EventId equals u.EventId
                                  where e.EventId == id
                                  select u);
                    Event original = query.FirstOrDefault();
                    EventUser original2 = query2.FirstOrDefault();
                    original2.IsAttend = eventUser.IsAttend;
                    
                if (userid == original2.UserId)
                {
                    if (original2.IsAttend == "Yes" && original.IsTicket == "No")
                    {
                        if ((original.Capacity != 0))
                        {
                            _context.SaveChanges();
                            return StatusCode(200, "You have been added to this event.");
                        }
                        else
                        {
                            return StatusCode(500, "We are sorry it's already full.");
                        }
                    }

                    else if (original2.IsAttend == "Yes" && original.IsTicket == "Yes")
                    {
                        if ((original.Capacity != 0))
                        {
                            _context.SaveChanges();
                            return Redirect("http://www.biletix.com");
                        }
                        else
                        {
                            return StatusCode(500, "We are sorry it's already full.");
                        }
                    }

                    else if (original2.IsAttend == "No" && original.IsTicket == "No")
                    {
                        if ((original.Date - DateTime.Now).TotalDays >= 2)
                        {
                            _context.SaveChanges();
                            return StatusCode(200, "You have been removed from this event.");
                        }
                        else
                        {
                            return StatusCode(500, "Less than 2 days left");
                        }
                    }
                    else if (original2.IsAttend == "No" && original.IsTicket == "Yes")
                    {
                        if ((original.Date - DateTime.Now).TotalDays >= 2)
                        {
                            _context.SaveChanges();
                            return Redirect("http://www.biletix.com");
                        }
                        else
                        {
                            return StatusCode(500, "Less than 2 days left");
                        }
                    }


                    else
                    {
                        return StatusCode(500, "Server error.");
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
       


        [HttpGet]
        public IActionResult GetEventsIsAttend()
        {
            try
            {
                List<AttendDTO> data = (from e in _context.EventUsers
                                        join u in _context.Users on e.UserId equals u.Id
                                        join o in _context.Events on e.EventId equals o.EventId
                                        where e.IsAttend == "Yes"
                                        select new AttendDTO()
                                        {
                                            EventName = o.EventName,
                                        }).Distinct().ToList();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }


        }
        [HttpGet]
        public IActionResult GetEventsNoAttend()
        {
            try
            {
                List<AttendDTO> data = (from e in _context.EventUsers
                                        join u in _context.Users on e.UserId equals u.Id
                                        join o in _context.Events on e.EventId equals o.EventId
                                        where e.IsAttend == "No"
                                        select new AttendDTO()
                                        {
                                            EventName = o.EventName,
                                        }).Distinct().ToList();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }


        }

        public IActionResult GetEventsCanceled()
        {
            try
            {
                List<AttendDTO> data = (from e in _context.Events
                                        where e.IsCanceled == "Yes"
                                        select new AttendDTO()
                                        {
                                            EventName = e.EventName,
                                        }).ToList();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }


        }


    }
}
