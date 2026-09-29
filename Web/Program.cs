using System.Text;
using System.Threading.RateLimiting;
using Domain.Config;
using Domain.Dto;
using Domain.Models;
using EvolveDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Service.BackgroundService;
using Service.Implementation;
using Service.Interface;
using Web.DbSeeder;
using Web.Interceptor;
using Web.Mapper;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseSqlite(connectionString);
    options.UseLazyLoadingProxies();
    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Concert & Music Platform API",
        Version = "v1",
        Description = "Concerts, artists, venues and ticketing. Use /api/auth/login to get a JWT, " +
                      "then click Authorize and paste it. External endpoints need the X-Api-Key header."
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste the JWT from /api/auth/login (no 'Bearer ' prefix needed)."
    });

    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "API key for /api/external/* endpoints (seeded: concert-external-key-123)."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();

builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IConcertService, ConcertService>();
builder.Services.AddScoped<ITicketCategoryService, TicketCategoryService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

builder.Services.AddScoped<IEtlSyncService, EtlSyncService>();
builder.Services.AddScoped<IInboundEventEntryService, InboundEventEntryService>();
builder.Services.AddScoped<IInboundEventEntryProcessor, InboundEventEntryProcessor>();

builder.Services.AddScoped<ArtistMapper>();
builder.Services.AddScoped<VenueMapper>();
builder.Services.AddScoped<ConcertMapper>();
builder.Services.AddScoped<TicketCategoryMapper>();
builder.Services.AddScoped<TicketMapper>();
builder.Services.AddScoped<PerformanceMapper>();

builder.Services.AddHostedService<SyncArtistsBackgroundService>();
builder.Services.AddHostedService<ProcessInboundEventsBackgroundService>();

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("RateLimitSettings"));
builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKeySettings"));
builder.Services.Configure<MusicApiSettings>(builder.Configuration.GetSection("MusicApiSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddHttpClient<IMusicApiClient, ItunesMusicApiClient>((sp, option) =>
{
    var settings = sp.GetRequiredService<IOptions<MusicApiSettings>>().Value;
    if (!string.IsNullOrWhiteSpace(settings.BaseAddress))
        option.BaseAddress = new Uri(settings.BaseAddress);
    option.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("external-api", context =>
    {
        var apiKey = context.Request.Headers["X-Api-Key"].ToString();
        var settings = context.RequestServices.GetRequiredService<IOptions<RateLimitSettings>>().Value;

        return RateLimitPartition.GetFixedWindowLimiter(apiKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = settings.PermitLimit,
            Window = TimeSpan.FromSeconds(settings.WindowInSeconds),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
});

builder.Services.AddIdentity<ConcertApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    try
    {
        using var cnx = new SqliteConnection(connectionString);
        var evolve = new Evolve(cnx, msg => app.Logger.LogInformation("{EvolveMsg}", msg))
        {
            Locations = new[] { "Database/Migrations" },
            IsEraseDisabled = true
        };
        evolve.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Evolve migration step skipped.");
    }

    if (app.Environment.IsDevelopment())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ConcertApplicationUser>>();
        await DbSeeder.SeedAsync(context, userManager);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Concert & Music Platform API v1");
        c.DocumentTitle = "Concerts API — Swagger";
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api", () => Results.Json(new
{
    application = "Concert & Music Platform API",
    status = "running",
    swaggerUi = "/swagger",
    endpoints = new[]
    {
        "GET  /api/artist", "GET /api/venue", "GET /api/concert", "GET /api/ticketcategory",
        "GET  /api/ticket/paged", "GET /api/performance/paged",
        "GET  /api/report/revenue", "GET /api/report/revenue/excel", "POST /api/report/etl/run",
        "POST /api/auth/register", "POST /api/auth/login",
        "POST /api/ticket/buy", "PATCH /api/ticket/{id}/pay", "PATCH /api/ticket/{id}/cancel",
        "GET  /api/ticket/{id}/qr-code", "POST /api/ticket/check-in/scan",
        "POST /api/external/tickets/register  (needs X-Api-Key header)"
    }
}));

app.Run();

public partial class Program
{
}
