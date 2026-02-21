using CleanArchitecture.Api.Middleware;
using CleanArchitecture.API.Extensions;
using MediatR;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Service;
using CleanArchitecture.Infrastructure.Context;
using CleanArchitecture.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.ConfigureSwagger();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Custom Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITokenService, CleanArchitecture.Infrastructure.Repository.TokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserServices, CleanArchitecture.Infrastructure.Repository.UserServiceImpl>();
builder.Services.AddScoped<IUnitOfWork, CleanArchitecture.Infrastructure.Repository.UnitOfWork>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CleanArchitecture.Application.IRepository.IMovieRepository).Assembly));

// Identity & JWT
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);

// CORS
builder.Services.ConfigureCors();

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();