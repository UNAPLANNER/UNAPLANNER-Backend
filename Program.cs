
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using UNAPLANNER_API.Data;
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
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<ICampusContactRepository, CampusContactRepository>();
builder.Services.AddScoped<ICampusContactService, CampusContactService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();

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
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Seed Roles if they don't exist
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { TypeRole = "Student" },
            new Role { TypeRole = "Admin" }
        );
        context.SaveChanges();
    }

    if (!context.Campuses.Any())
    {
        context.Campuses.AddRange(
            new Campus { Name = "Campus Sarapiquí", Code = "SAR", IsStatus = true, CreatedDate = DateTime.Now },
            new Campus { Name = "Campus Liberia", Code = "LIB", IsStatus = true, CreatedDate = DateTime.Now },
            new Campus { Name = "Campus Nicoya", Code = "NIC", IsStatus = true, CreatedDate = DateTime.Now },
            new Campus { Name = "Sede Central", Code = "CEN", IsStatus = true, CreatedDate = DateTime.Now }
        );
        context.SaveChanges();
    }

    var adminUser = context.Users.FirstOrDefault(u => u.Email == adminEmail);

    if (adminUser == null)
    {
        adminUser = new User
        {
            Email = adminEmail!,
            Password = BCrypt.Net.BCrypt.HashPassword(adminPassword!),
            RoleId = RoleContants.Admin,
            IsStatus = true,
            CreatedDate = DateTime.Now
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }

    var sarapiquiCampusId = context.Campuses.First(c => c.Code == "SAR").Id;
    var adminProfile = context.Admins.FirstOrDefault(a => a.UserId == adminUser.UserId);

    if (adminProfile == null)
    {
        context.Admins.Add(new Admin
        {
            UserId = adminUser.UserId,
            FullName = "Administrador General",
            Department = "Registro Financiero Sarapiquí",
            Phone = "2277-3000",
            CampusId = sarapiquiCampusId
        });
    }
    else
    {
        adminProfile.FullName = "Administrador General";
        adminProfile.Department = "Registro Financiero Sarapiquí";
        adminProfile.Phone = "2277-3000";
        adminProfile.CampusId = sarapiquiCampusId;
    }

    context.SaveChanges();
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
