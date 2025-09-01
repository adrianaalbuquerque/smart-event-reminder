using Microsoft.AspNetCore.Mvc;
using SmartEventReminder.Domain;
using SmartEventReminder.Domain.Models;
using SmartEventReminder.Domain.DTOs;

namespace SmartEventReminder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Event>> GetEvents()
        {
            var events = _context.Events.ToList();
            return Ok(events);
            
        }

        [HttpPost]
        public ActionResult CreateEvent(EventDTO dto)
        {
            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return Ok(newEvent);
        }
    }
}
