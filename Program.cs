
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using UNAPLANNER_API.Data;
using UNAPLANNER_API.Infrastructure.Firebase;
using UNAPLANNER_API.Repositories;
using UNAPLANNER_API.Services;
using UNAPLANNER_API.Constants;
using UNAPLANNER_API.Models.Entities;


var builder = WebApplication.CreateBuilder(args);

//appsentings password and email
var adminEmail = builder.Configuration["AdminSettings:Email"];
var adminPassword = builder.Configuration["AdminSettings:Password"];

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// Repositories & Services
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<ICampusContactRepository, CampusContactRepository>();
builder.Services.AddScoped<ICampusContactService, CampusContactService>();
builder.Services.AddScoped<IStudentGPARepository, StudentGPARepository>();
builder.Services.AddScoped<IStudentGPAService, StudentGPAService>();
builder.Services.AddScoped<IUserStudentRepository, UserStudentRepository>();
builder.Services.AddScoped<IUserStudentService, UserStudentService>();
builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
builder.Services.AddScoped<IEvaluationService, EvaluationService>();
builder.Services.AddScoped<ICareerRepository, CareerRepository>();
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<IStudyPlanRepository, StudyPlanRepository>();
builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<ICurriculumService, CurriculumService>();
builder.Services.AddScoped<IStudyPlanService, StudyPlanService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

// Notifications
builder.Services.AddSingleton<INotificationSender, FirebaseNotificationSender>();
builder.Services.AddScoped<INotificationTokenRepository, NotificationTokenRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Notifications
builder.Services.AddSingleton<INotificationSender, FirebaseNotificationSender>();
builder.Services.AddScoped<INotificationTokenRepository, NotificationTokenRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UNAPlanner API",
        Version = "v1",
        Description = "API del sistema UNAPlanner"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Escribe: Bearer {tu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    c.EnableAnnotations();
});

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

//email y contraseña del admin por defecto
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext >();

    if (!context.Users.Any(u => u.Email == adminEmail))
    {
        var admin = new User
        {
            Email = adminEmail!,
            Password = BCrypt.Net.BCrypt.HashPassword(adminPassword!),
            RoleId = RoleContants.Admin,
            IsStatus = true,
            CreatedDate = DateTime.Now
        };

        context.Users.Add(admin);
        context.SaveChanges();
    }

    var adminUser = context.Users.FirstOrDefault(u => u.Email == adminEmail);
    if (adminUser != null && !context.Admins.Any(a => a.UserId == adminUser.UserId))
    {
        var campusId = context.Campuses.Select(c => c.Id).FirstOrDefault();
        if (campusId != 0)
        {
            context.Admins.Add(new Admin
            {
                UserId = adminUser.UserId,
                FullName = "Administrador UNAPlanner",
                Department = "Administracion",
                CampusId = campusId
            });
            context.SaveChanges();
        }
    }
}


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
