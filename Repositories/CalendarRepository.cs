using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly AppDbContext _context;

    public CalendarRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Calendar>> GetEventsByUserIdAsync(int userId)
    {
        return await _context.Calendars
            .Include(c => c.Course)
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.ActivityDate)
            .ToListAsync();
    }

    public async Task<Calendar?> GetByIdAsync(int id)
    {
        return await _context.Calendars
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Calendar>> GetEventsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Calendars
            .Include(c => c.Course)
            .Where(c => c.UserId == userId && c.ActivityDate >= startDate && c.ActivityDate <= endDate)
            .OrderBy(c => c.ActivityDate)
            .ToListAsync();
    }

    public async Task<List<Calendar>> GetEventsByActivityTypeAsync(int userId, string activityType)
    {
        return await _context.Calendars
            .Include(c => c.Course)
            .Where(c => c.UserId == userId && c.ActivityType == activityType)
            .OrderBy(c => c.ActivityDate)
            .ToListAsync();
    }

    public async Task<Calendar> CreateAsync(Calendar calendarEvent)
    {
        _context.Calendars.Add(calendarEvent);
        await _context.SaveChangesAsync();
        return calendarEvent;
    }

    public async Task<bool> CourseExistsAsync(int courseId)
    {
        return await _context.Courses.AnyAsync(c => c.Id == courseId);
    }

    public async Task<Calendar> UpdateAsync(Calendar calendarEvent)
    {
        _context.Calendars.Update(calendarEvent);
        await _context.SaveChangesAsync();
        return calendarEvent;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var calendarEvent = await _context.Calendars.FindAsync(id);
        if (calendarEvent == null) return false;
        _context.Calendars.Remove(calendarEvent);
        await _context.SaveChangesAsync();
        return true;
    }
}
