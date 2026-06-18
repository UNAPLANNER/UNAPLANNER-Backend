using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Models.Entities;

public interface ICampusRepository
{
    //Get the complete list of campuses
    Task<List<Campus>> GetAllCampus();

    // Retrieves a campus by its unique identifier.
    Task<Campus?> GetByIdCampus(int id);
}