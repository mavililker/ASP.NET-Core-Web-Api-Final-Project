using EventProjectFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace EventProjectFinal.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly EVENTS3Context _context;

        public AdminController(EVENTS3Context context)
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

        [HttpPost]
        
        public IActionResult AddCity([FromBody] City city)
        {

            try
            {
                var query = (from e in _context.Cities where e.CityName == city.CityName select e);
                City original = query.SingleOrDefault();
                if (original == null)
                {
                    _context.Cities.Add(city);
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
        
        [HttpPost]
        public IActionResult AddCategory([FromBody] Category category)
        {
            try
            {
                var query = (from e in _context.Categories where e.CategoryName == category.CategoryName select e);
                Category original = query.SingleOrDefault();
                if (original  == null)
                {
                    _context.Categories.Add(category);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return StatusCode(400, "Alreadys exists.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }


        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            
            try
            {
                var query = (from e in _context.Categories where e.CategoryId == id select e);
                Category category = query.SingleOrDefault();
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Ok();
            }

            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }

        }
        
        [HttpPut("{id}")]

        public IActionResult UpdateCategory(int id, Category category)
        {
            try
            {
                var query = (from e in _context.Categories where e.CategoryId == id select e);
                Category original = query.SingleOrDefault();

                var query2 = (from e in _context.Categories where e.CategoryName == category.CategoryName select e);
                Category original2 = query2.SingleOrDefault();

                if (original2 == null)
                {
                    original.CategoryName = category.CategoryName;
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return StatusCode(400, "Alreadys exists.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Server error.");
            }


        }
    }
}
