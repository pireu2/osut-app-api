using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public class EventService(
    IEventRepository eventRepository,
    IEventSignupRepository signupRepository,
    IDepartmentRepository departmentRepository) : IEventService
{
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IEventSignupRepository _signupRepository = signupRepository;
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        return await _eventRepository.GetUpcomingAsync();
    }

    public async Task<Event?> GetEventByIdAsync(Guid id)
    {
        return await _eventRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Event>> GetEventsByDepartmentAsync(Guid departmentId)
    {
        return await _eventRepository.GetByDepartmentAsync(departmentId);
    }

    public async Task<Event> CreateEventAsync(string title, string? description, DateTime dateTime, string location, Guid departmentId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new ArgumentException("Invalid department ID");
        }

        var eventEntity = new Event
        {
            Title = title,
            Description = description,
            DateTime = dateTime,
            Location = location,
            DepartmentId = departmentId
        };

        await _eventRepository.AddAsync(eventEntity);

        return eventEntity;
    }

    public async Task<Event?> UpdateEventAsync(Guid id, string? title, string? description, DateTime? dateTime, string? location)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);

        if (eventEntity == null)
        {
            return null;
        }

        if (title != null)
        {
            eventEntity.Title = title;
        }

        if (description != null)
        {
            eventEntity.Description = description;
        }

        if (dateTime.HasValue)
        {
            eventEntity.DateTime = dateTime.Value;
        }

        if (location != null)
        {
            eventEntity.Location = location;
        }

        await _eventRepository.UpdateAsync(eventEntity);

        return eventEntity;
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

    public async Task<IEnumerable<EventSignup>> GetEventSignupsAsync(Guid eventId)
    {
        return await _signupRepository.GetByEventAsync(eventId);
    }

    public async Task<IEnumerable<EventSignup>> GetUserSignupsAsync(string userId)
    {
        return await _signupRepository.GetByUserAsync(userId);
    }
}