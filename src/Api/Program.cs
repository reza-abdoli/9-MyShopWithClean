// Api/Program.cs
using System.Text;
using System.Text.Json.Serialization;
using Api.Common;
using Api.Extensions;
using Application;
using Application.Features.Payments;          // جدید
using FileStorage;
using Infrastructure.Sms;                     // جدید
using JasperFx.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Persistence;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.SqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLog4Net("log4net.config");

// -------------------- Wolverine --------------------
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(ApplicationAssemblyMarker).Assembly);
    opts.CodeGeneration.AlwaysUseServiceLocationFor<ApplicationDbContext>();

    opts.PersistMessagesWithSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!, "wolverine");

    opts.UseEntityFrameworkCoreTransactions();

    // پیام‌های صف محلی (مثل SendSmsCommand) توی دیتابیس پایدار می‌شن
    opts.Policies.UseDurableLocalQueues();

    // حذف شد: AddDbContextWithWolverineIntegration (رفت توی Persistence)
    // حذف شد: AutoApplyTransactions (commit رو صریح خودمون انجام می‌دیم)
});

// جدول‌های schema به اسم wolverine رو موقع استارت می‌سازه
builder.Services.AddResourceSetupOnStartup();

// -------------------- Controllers --------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// -------------------- لایه‌های Infrastructure --------------------
builder.Services.AddMemoryCache();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddFileStorageServices();
builder.Services.AddSmsServices();                        // جدید
builder.Services.AddScoped<OnlinePaymentRefunder>();      // جدید

// -------------------- JWT Authentication --------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", policy =>
    {
        policy.WithOrigins(builder.Configuration["UI:BaseUrl"] ?? "https://localhost:7100")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------- Swagger --------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyMvcApp Api", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "توکن JWT را با فرمت زیر وارد کنید: Bearer {token}"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyMvcApp Api v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowUI");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.SeedDataAsync();

app.Run();