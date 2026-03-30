using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackAPI.Models;
using TrackAPI.Services;

namespace TrackAPI.Controllers
{
    [Route("api/event")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EventDto>> Get()
        {
            var events = _eventService.GetEvents();

            return Ok(events);
        }

        [HttpGet("byid")]
        public ActionResult<EventDto> GetById([FromQuery] int id)
        {
            var eventById = _eventService.GetEventById(id);

            return Ok(eventById);
        }

        [HttpPost("add")]
        public ActionResult AddEvent([FromBody] AddEventDto eventDto)
        {
            var newEvent = _eventService.AddEvent(eventDto);

            return Ok(newEvent);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult EditEvent([FromBody] EditEventDto eventDto, [FromRoute] int id)
        {
            _eventService.EditEvent(id, eventDto);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteEvent([FromRoute] int id)
        {
            _eventService.DeleteEvent(id);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public ActionResult ApproveEvent([FromRoute] int id)
        {
            var result = _eventService.ApproveEvent(id);
            if (!result) return NotFound();

            return Ok();
        }
    }
}
