using AutoMapper;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public class EventService(
    IEventRepository eventRepository,
    IEventSignupRepository signupRepository,
    IDepartmentRepository departmentRepository,
    IMapper mapper) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IEventSignupRepository _signupRepository = signupRepository;
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
    {
        var events = await _eventRepository.GetUpcomingAsync();

        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);

        return eventEntity != null ? _mapper.Map<EventDto>(eventEntity) : null;
    }

    public async Task<IEnumerable<EventDto>> GetEventsByDepartmentAsync(Guid departmentId)
    {
        var events = await _eventRepository.GetByDepartmentAsync(departmentId);

        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<EventDto> CreateEventAsync(EventDto eventDto)
    {
        var department = await _departmentRepository.GetByIdAsync(eventDto.DepartmentId);

        if (department == null)
        {
            throw new ArgumentException("Invalid department ID");
        }

        var eventEntity = new Event
        {
            Title = eventDto.Title,
            Description = eventDto.Description,
            DateTime = eventDto.DateTime,
            Location = eventDto.Location,
            DepartmentId = eventDto.DepartmentId
        };

        await _eventRepository.AddAsync(eventEntity);

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<EventDto?> UpdateEventAsync(Guid id, EventDto eventDto)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);

        if (eventEntity == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(eventDto.Title))
        {
            eventEntity.Title = eventDto.Title;
        }

        if (eventDto.Description != null)
        {
            eventEntity.Description = eventDto.Description;
        }

        if (eventDto.DateTime != default(DateTime))
        {
            eventEntity.DateTime = eventDto.DateTime;
        }

        if (!string.IsNullOrEmpty(eventDto.Location))
        {
            eventEntity.Location = eventDto.Location;
        }

        await _eventRepository.UpdateAsync(eventEntity);

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);

        if (eventEntity == null)
        {
            return false;
        }

        await _eventRepository.DeleteAsync(eventEntity);

        return true;
    }

    public async Task<bool> SignupForEventAsync(Guid eventId, string userId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);

        if (eventEntity == null)
        {
            throw new ArgumentException("Event not found");
        }

        var existingSignup = await _signupRepository.GetByEventAndUserAsync(eventId, userId);

        if (existingSignup != null)
        {
            throw new InvalidOperationException("Already signed up for this event");
        }

        var signup = new EventSignup
        {
            EventId = eventId,
            UserId = userId
        };

        await _signupRepository.AddAsync(signup);

        return true;
    }

    public async Task<bool> CancelSignupAsync(Guid eventId, string userId)
    {
        var signup = await _signupRepository.GetByEventAndUserAsync(eventId, userId);

        if (signup == null)
        {
            return false;
        }

        await _signupRepository.DeleteAsync(signup);

        return true;
    }

    public async Task<IEnumerable<EventSignupDto>> GetEventSignupsAsync(Guid eventId)
    {
        var signups = await _signupRepository.GetByEventAsync(eventId);

        return _mapper.Map<IEnumerable<EventSignupDto>>(signups);
    }

    public async Task<IEnumerable<EventSignupDto>> GetUserSignupsAsync(string userId)
    {
        var signups = await _signupRepository.GetByUserAsync(userId);

        return _mapper.Map<IEnumerable<EventSignupDto>>(signups);
    }
}