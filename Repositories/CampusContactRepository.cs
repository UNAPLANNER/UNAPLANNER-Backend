using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Repositories;

public class CampusContactRepository : ICampusContactRepository
{
    private readonly AppDbContext _context;

    public CampusContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CampusContact>> GetAllAsync()
    {
        return await _context.CampusContacts
            .Where(cc => cc.IsStatus)
            .OrderBy(cc => cc.DepartamentName)
            .ToListAsync();
    }

    public async Task<List<CampusContact>> GetByCampusIdAsync(int campusId)
    {
        return await _context.CampusContacts
            .Where(cc => cc.CampusId == campusId && cc.IsStatus)
            .OrderBy(cc => cc.DepartamentName)
            .ToListAsync();
    }

    public async Task<CampusContact?> GetByIdAsync(int id)
    {
        return await _context.CampusContacts
            .FirstOrDefaultAsync(cc => cc.Id == id && cc.IsStatus);
    }

    public async Task<CampusContact> AddAsync(CampusContact contact)
    {
        _context.CampusContacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var contact = await _context.CampusContacts
            .FirstOrDefaultAsync(cc => cc.Id == id && cc.IsStatus);

        if (contact == null)
        {
            return false;
        }

        contact.IsStatus = false;
        return await _context.SaveChangesAsync() > 0;
    }
}
