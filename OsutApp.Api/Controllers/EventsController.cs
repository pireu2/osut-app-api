using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsutApp.Api.DTOs;
using OsutApp.Api.Services;
using System.Security.Claims;

namespace OsutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController(
    IEventService eventService) : BaseController
{
    private readonly IEventService _eventService = eventService;

    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _eventService.GetAllEventsAsync();

        return Ok(events);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingEvents()
    {
        var events = await _eventService.GetUpcomingEventsAsync();

        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(Guid id)
    {
        var eventEntity = await _eventService.GetEventByIdAsync(id);

        if (eventEntity == null)
        {
            return NotFound();
        }

        return Ok(eventEntity);
    }

    [HttpGet("department/{departmentId}")]
    public async Task<IActionResult> GetEventsByDepartment(Guid departmentId)
    {
        var events = await _eventService.GetEventsByDepartmentAsync(departmentId);

        return Ok(events);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateEvent([FromBody] EventDto request)
    {
        try
        {
            var eventEntity = await _eventService.CreateEventAsync(request);

            return CreatedAtAction(nameof(GetEvent), new { id = eventEntity.Id }, eventEntity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventDto request)
    {
        var eventEntity = await _eventService.UpdateEventAsync(id, request);

        if (eventEntity == null)
        {
            return NotFound();
        }

        return Ok(eventEntity);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var success = await _eventService.DeleteEventAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{eventId}/signup")]
    public async Task<IActionResult> SignupForEvent(Guid eventId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            await _eventService.SignupForEventAsync(eventId, currentUserId.ToString());

            return Ok("Successfully signed up for the event");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex) when (ex.Message == "Event not found")
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Already signed up for this event")
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{eventId}/signup")]
    public async Task<IActionResult> CancelSignup(Guid eventId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var success = await _eventService.CancelSignupAsync(eventId, currentUserId.ToString());

            if (!success)
            {
                return NotFound("Signup not found");
            }

            return Ok("Signup cancelled");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("{eventId}/signups")]
    [Authorize]
    public async Task<IActionResult> GetEventSignups(Guid eventId)
    {
        var signups = await _eventService.GetEventSignupsAsync(eventId);

        return Ok(signups);
    }
}