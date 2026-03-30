using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrackAPI.Entities;
using TrackAPI.Migrations;
using TrackAPI.Models;

namespace TrackAPI.Services
{
    public interface IEventService
    {
        IEnumerable<EventDto> GetEvents();
        EventDto GetEventById(int id);
        int AddEvent(AddEventDto dto);
        bool DeleteEvent(int id);
        bool EditEvent(int id, EditEventDto dto);
        bool ApproveEvent(int id);
    }
    public class EventService : IEventService
    {
        private readonly TrackDbContext _db;
        private readonly IMapper _mapper;
        public EventService(TrackDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public IEnumerable<EventDto> GetEvents()
        {
            var events = _db.Events
                .Include(e => e.Organizer)
                .ToList();

            var eventMap = _mapper.Map<List<EventDto>>(events);

            return eventMap;
        }

        public EventDto GetEventById(int id)
        {
            var Event = _db.Events
                .Include(e => e.Organizer)
                .FirstOrDefault(e => e.Id == id);

            var eventMap = _mapper.Map<EventDto>(Event);

            return eventMap;
        }

        public int AddEvent(AddEventDto dto)
        {
            var newEvent = _mapper.Map<Event>(dto);


            _db.Events.Add(newEvent);
            _db.SaveChanges();

            return newEvent.Id;
        }

        public bool DeleteEvent(int id)
        {
            var eventToDelete = _db.Events.FirstOrDefault(e => e.Id == id);
            _db.Events.Remove(eventToDelete);
            _db.SaveChanges();

            return true;
        }

        public bool EditEvent(int id, EditEventDto dto)
        {
            var eventToEdit = _db.Events.FirstOrDefault(e => e.Id == id);

            if (eventToEdit == null)
            {
                return false;
            }

            if(dto.EventName != null)
            {
                eventToEdit.EventName = dto.EventName;
            }

            if(dto.Description != null)
            {
                eventToEdit.Description = dto.Description;
            }

            if(dto.StartDate != default(DateTime))
            {
                eventToEdit.StartDate = dto.StartDate;
            }

            if (dto.EndDate != default(DateTime))
            {
                eventToEdit.EndDate = dto.EndDate;
            }

            if(dto.EventType != 0)
            {
                eventToEdit.EventType = dto.EventType;
            }

            _db.SaveChanges();

            return true;
        }

        public bool ApproveEvent(int id)
        {
            var eventToApprove = _db.Events.FirstOrDefault(e => e.Id == id);

            if (eventToApprove == null)
            {
                return false;
            }

            eventToApprove.EventStatus = 2;
            _db.SaveChanges();
            return true;
        }
    }
}
