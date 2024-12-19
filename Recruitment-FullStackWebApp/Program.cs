using Microsoft.Data.SqlClient;
using System.Data;
using AutoMapper;
using System.Reflection;
using Dapper;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Dependency Injection using Reflection
// Find all classes that end with "Repository" and implement an interface
var serviceTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Service"))
    .ToList();

foreach (var implementationType in serviceTypes)
{
    // Find the interface that matches the naming convention (IServiceName)
    var interfaceType = implementationType.GetInterface($"I{implementationType.Name}");
    if (interfaceType != null)
    {
        // Register the service with Scoped lifetime
        builder.Services.AddScoped(interfaceType, implementationType);
    }
}

var repositoryTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Repository"))
    .ToList();

foreach (var implementationType in repositoryTypes)
{
    // Find the interface that matches the naming convention (IServiceName)
    var interfaceType = implementationType.GetInterface($"I{implementationType.Name}");
    if (interfaceType != null)
    {
        // Register the service with Scoped lifetime
        builder.Services.AddScoped(interfaceType, sp =>
        {
            // Get the connection string from configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Get the AutoMapper instance from the DI container
            var mapper = sp.GetRequiredService<IMapper>();

            // Create the repository and pass required dependencies (connection string and IMapper)
            return Activator.CreateInstance(implementationType, connectionString, mapper);
        });
    }
}


// Add Database Connection
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero 
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Authentication middleware (if you're using JWT or cookie-based auth)
app.UseAuthentication();
app.UseAuthorization();

// Configure custom middlewares if necessary (e.g., to handle unauthorized access globally)
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    var request = context.HttpContext.Request;

    // Check for Unauthorized (401) status code and redirect to login
    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        response.Redirect("/User/Login");
    }

    await Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
