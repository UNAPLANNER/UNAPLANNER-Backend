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
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<AdminLog> AdminLogs => Set<AdminLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== UNIQUE CONSTRAINTS ==========

        // Role
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.TypeRole)
            .IsUnique();

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Campus
        modelBuilder.Entity<Campus>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // Career
        modelBuilder.Entity<Career>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // StudyPlan
        modelBuilder.Entity<StudyPlan>()
            .HasIndex(sp => sp.Code)
            .IsUnique();

        // Student
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.UserId)
            .IsUnique();

        // Course
        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // StudyPlanCourse [FIX-2]
        modelBuilder.Entity<StudyPlanCourse>()
            .HasIndex(spc => new { spc.StudyPlanId, spc.CourseId })
            .IsUnique();

        // Requirement [FIX-3]
        modelBuilder.Entity<Requirement>()
            .HasIndex(r => new { r.CourseId, r.RequiredCourseId })
            .IsUnique();

        // StudentProgress
        modelBuilder.Entity<StudentProgress>()
            .HasIndex(sp => new { sp.StudentId, sp.CourseId })
            .IsUnique();

        // StudentCourseDetail [FIX-4]
        modelBuilder.Entity<StudentCourseDetail>()
            .HasIndex(scd => scd.StudentProgressId)
            .IsUnique();

        // NotificationToken
        modelBuilder.Entity<NotificationToken>()
            .HasIndex(nt => new { nt.UserId, nt.FcmToken })
            .IsUnique();

        // ========== RELATIONSHIPS ==========

        // User → Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Career → Campus
        modelBuilder.Entity<Career>()
            .HasOne(c => c.Campus)
            .WithMany(ca => ca.Careers)
            .HasForeignKey(c => c.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        // StudyPlan → Career
        modelBuilder.Entity<StudyPlan>()
            .HasOne(sp => sp.Career)
            .WithMany(c => c.StudyPlans)
            .HasForeignKey(sp => sp.CareerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student → User
        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.Student)   
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithOne(u => u.Admin)
            .HasForeignKey<Admin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Admin>()
            .HasOne(a => a.Campus)
            .WithMany(c => c.Admins)
            .HasForeignKey(a => a.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student → Career
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Career)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CareerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student → StudyPlan
        modelBuilder.Entity<Student>()
            .HasOne(s => s.StudyPlan)
            .WithMany(sp => sp.Students)
            .HasForeignKey(s => s.StudyPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // StudyPlanCourse → StudyPlan
        modelBuilder.Entity<StudyPlanCourse>()
            .HasOne(spc => spc.StudyPlan)
            .WithMany(sp => sp.StudyPlanCourses)
            .HasForeignKey(spc => spc.StudyPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // StudyPlanCourse → Course
        modelBuilder.Entity<StudyPlanCourse>()
            .HasOne(spc => spc.Course)
            .WithMany(c => c.StudyPlanCourses)
            .HasForeignKey(spc => spc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Requirement → Course (CourseId)
        modelBuilder.Entity<Requirement>()
            .HasOne(r => r.Course)
            .WithMany(c => c.CourseRequirements)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Requirement → Course (RequiredCourseId)
        modelBuilder.Entity<Requirement>()
            .HasOne(r => r.RequiredCourse)
            .WithMany(c => c.RequiredByRequirements)
            .HasForeignKey(r => r.RequiredCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentProgress → Student
        modelBuilder.Entity<StudentProgress>()
            .HasOne(sp => sp.Student)
            .WithMany(s => s.StudentProgress)
            .HasForeignKey(sp => sp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // StudentProgress → Course
        modelBuilder.Entity<StudentProgress>()
            .HasOne(sp => sp.Course)
            .WithMany(c => c.StudentProgress)
            .HasForeignKey(sp => sp.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentCourseDetail → StudentProgress
        modelBuilder.Entity<StudentCourseDetail>()
            .HasOne(scd => scd.StudentProgress)
            .WithOne(sp => sp.StudentCourseDetail)
            .HasForeignKey<StudentCourseDetail>(scd => scd.StudentProgressId)
            .OnDelete(DeleteBehavior.Cascade);

        // Evaluation → StudentProgress
        modelBuilder.Entity<Evaluation>()
            .HasOne(e => e.StudentProgress)
            .WithMany(sp => sp.Evaluations)
            .HasForeignKey(e => e.StudentProgressId)
            .OnDelete(DeleteBehavior.Cascade);

        // Calendar → User
        modelBuilder.Entity<Calendar>()
            .HasOne(c => c.User)
            .WithMany(u => u.Calendars)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Calendar → Course
        modelBuilder.Entity<Calendar>()
            .HasOne(c => c.Course)
            .WithMany(co => co.Calendars)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        // File → User
        modelBuilder.Entity<UNAPLANNER_API.Models.Entities.File>()
            .HasOne(f => f.User)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // File → Course
        modelBuilder.Entity<UNAPLANNER_API.Models.Entities.File>()
            .HasOne(f => f.Course)
            .WithMany(c => c.Files)
            .HasForeignKey(f => f.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        // Note → User
        modelBuilder.Entity<Note>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Note → Course
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Course)
            .WithMany(c => c.Notes)
            .HasForeignKey(n => n.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        // CampusContact → Campus
        modelBuilder.Entity<CampusContact>()
            .HasOne(cc => cc.Campus)
            .WithMany(c => c.CampusContacts)
            .HasForeignKey(cc => cc.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        // NotificationToken → User
        modelBuilder.Entity<NotificationToken>()
            .HasOne(nt => nt.User)
            .WithMany(u => u.NotificationTokens)
            .HasForeignKey(nt => nt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notification → User
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // AdminLog → User
        modelBuilder.Entity<AdminLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AdminLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Evaluation>()
            .Property(e => e.Percentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Evaluation>()
            .Property(e => e.Score)
            .HasPrecision(5, 2);

        modelBuilder.Entity<StudentProgress>()
            .Property(sp => sp.FinalGrade)
            .HasPrecision(5, 2);


        // ========== CHECK CONSTRAINTS & CONFIGURATIONS ==========
        // Requirement
        modelBuilder.Entity<Requirement>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_Requirement_Type",
                "RequirementType IN ('Prerequisite', 'Corequisite')");
                t.HasCheckConstraint("CK_Requirement_NoSelfCycle",
                "CourseId <> RequiredCourseId");
            });
        // StudentProgress
        modelBuilder.Entity<StudentProgress>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_StudentProgress_Status",
                    "Status IN ('Pendiente', 'EnCurso', 'Aprobado', 'Reprobado')");
                t.HasCheckConstraint("CK_StudentProgress_Grade",
                    "FinalGrade IS NULL OR (FinalGrade >= 0 AND FinalGrade <= 100)");
                t.HasCheckConstraint("CK_StudentProgress_Term",
                    "AcademicTerm IS NULL OR AcademicTerm IN (1, 2)");
            });
        // StudyPlanCourse
        modelBuilder.Entity<StudyPlanCourse>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_StudyPlanCourse_Levels",
                    "Levels BETWEEN 1 AND 6");
                t.HasCheckConstraint("CK_StudyPlanCourse_Term",
                    "Term IN (1, 2)");
            });

        // Evaluation
        modelBuilder.Entity<Evaluation>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_Evaluation_Percentage",
                    "Percentage >= 0.01 AND Percentage <= 100");
                t.HasCheckConstraint("CK_Evaluation_Score",
                    "Score IS NULL OR (Score >= 0 AND Score <= 100)");
                t.HasCheckConstraint("CK_Evaluation_Type",
                    "EvaluationType IN ('Examen', 'Tarea', 'Proyecto', 'Quiz', 'Exposicion', 'Otro')");
            });
        // Calendar
        modelBuilder.Entity<Calendar>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_Calendar_Type",
                    "ActivityType IN ('Examen', 'Tarea', 'Proyecto', 'Exposicion', 'Evento', 'Otro')");
                t.HasCheckConstraint("CK_Calendar_Reminder",
                    "HasReminder = 0 OR ReminderDate IS NOT NULL");
            });
        // File
        modelBuilder.Entity<UNAPLANNER_API.Models.Entities.File>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_File_Type",
                    "FileType IN ('PDF', 'JPG', 'PNG', 'DOCX', 'XLSX', 'TXT', 'OTRO')");
            });
        // Notification
        modelBuilder.Entity<Notification>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_Notification_Type",
                    "Type IN ('CourseApproved', 'ExamReminder', 'TaskReminder', 'ProjectReminder', 'EventReminder', 'General')");
            });
    }

}
