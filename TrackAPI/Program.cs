using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrackAPI;
using TrackAPI.Entities;
using TrackAPI.Middleware;
using TrackAPI.Models;
using TrackAPI.Services;
using VueApp1.Server.Models.Validators;
using VueApp1.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var authenticationsettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationsettings);

builder.Services.AddSingleton(authenticationsettings);
builder.Services.AddAuthentication(cfg =>
{
    cfg.DefaultScheme = "Bearer";
    cfg.DefaultAuthenticateScheme = "Bearer";
    cfg.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = authenticationsettings.JWTIssuer,
        ValidAudience = authenticationsettings.JWTIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationsettings.JWTKey))
    };
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterAccountValidator>();


builder.Services.AddDbContext<TrackDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<User, UserDto>()
        .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.RoleName));

    cfg.CreateMap<UserDto, User>();

    cfg.CreateMap<RegisterUserDto, User>();

    cfg.CreateMap<Event, EventDto>();

    cfg.CreateMap<AddEventDto, Event>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndClient", builder =>
        builder.AllowAnyMethod()
               .AllowAnyHeader()
               .WithOrigins("http://localhost:5173"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEndClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
