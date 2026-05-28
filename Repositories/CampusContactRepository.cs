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
            .Include(cc => cc.Campus)
            .Where(cc => cc.IsStatus)
            .OrderBy(cc => cc.DepartamentName)
            .ToListAsync();
    }

    public async Task<List<CampusContact>> GetByCampusIdAsync(int campusId)
    {
        return await _context.CampusContacts
            .Include(cc => cc.Campus)
            .Where(cc => cc.CampusId == campusId && cc.IsStatus)
            .OrderBy(cc => cc.DepartamentName)
            .ToListAsync();
    }

    public async Task<CampusContact?> GetByIdAsync(int id)
    {
        return await _context.CampusContacts
            .Include(cc => cc.Campus)
            .FirstOrDefaultAsync(cc => cc.Id == id && cc.IsStatus);
    }

    public async Task<CampusContact> CreateAsync(CampusContact campusContact)
    {
        _context.CampusContacts.Add(campusContact);
        await _context.SaveChangesAsync();
        return campusContact;
    }

    public async Task<CampusContact?> UpdateAsync(CampusContact campusContact)
    {
        var existingContact = await _context.CampusContacts
            .FirstOrDefaultAsync(cc => cc.Id == campusContact.Id && cc.IsStatus);
        
        if (existingContact == null)
            return null;

        // Update only the fields that can be modified
        existingContact.DepartamentName = campusContact.DepartamentName;
        existingContact.Phone = campusContact.Phone;
        existingContact.CampusId = campusContact.CampusId;
        existingContact.Email = campusContact.Email;
        existingContact.Description = campusContact.Description;

        _context.CampusContacts.Update(existingContact);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(existingContact.Id);
    }

    public async Task<bool> CampusExistsAsync(int campusId)
    {
        return await _context.Campuses.AnyAsync(c => c.Id == campusId && c.IsStatus);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var contact = await _context.CampusContacts.FirstOrDefaultAsync(cc => cc.Id == id);
        if (contact == null) return false;

        _context.CampusContacts.Remove(contact);
        await _context.SaveChangesAsync();
        return true;
    }
}
