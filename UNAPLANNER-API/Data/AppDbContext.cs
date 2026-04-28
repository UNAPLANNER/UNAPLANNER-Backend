using Microsoft.EntityFrameworkCore;
using UNAPLANNER_API.Models.Entities;

namespace UNAPLANNER_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<User> Users => Set<User>();
    // Campus & Career
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Career> Careers => Set<Career>();
    public DbSet<CampusContact> CampusContacts => Set<CampusContact>();

    // Study Plan
    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<StudyPlanCourse> StudyPlanCourses => Set<StudyPlanCourse>();

    // Student
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentProgress> StudentProgress => Set<StudentProgress>();
    public DbSet<StudentCourseDetail> StudentCourseDetails => Set<StudentCourseDetail>();

    // Course Management
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();

    // Calendar & Files
    public DbSet<Calendar> Calendars => Set<Calendar>();
    public DbSet<UNAPLANNER_API.Models.Entities.File> Files => Set<UNAPLANNER_API.Models.Entities.File>();
    public DbSet<Note> Notes => Set<Note>();

    // Notifications
    public DbSet<NotificationToken> NotificationTokens => Set<NotificationToken>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // Admin
    public DbSet<AdminLog> AdminLogs => Set<AdminLog>();

}
