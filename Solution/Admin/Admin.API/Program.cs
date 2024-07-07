using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using JaCaptei.Model;
using JaCaptei.Administrativo.API.Filters;
using System.Text.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JaCaptei.Admin.API;

//var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
//    ApplicationName = typeof(Program).Assembly.FullName,
//    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
//    WebRootPath = "UI",
//    Args = args
//});


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddEndpointsApiExplorer();  // to mvc application 
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddControllersWithViews(options => {
    options.Filters.Add<ExceptionFilter>();
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    //options.Filters.Add<ActionFilter>();
});

// Add services to the container.

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;



// --------------------------- CONFIGURATION SETTINGS ---------------------------

configuration.SetBasePath(environment.ContentRootPath)
        .AddJsonFile("appsettings.json",optional: true,reloadOnChange: true)
        .AddJsonFile("appsettings.Production.json",optional: true,reloadOnChange: true)
        .AddJsonFile("appsettings.Homolog.json",optional: true,reloadOnChange: true)
        .AddJsonFile("appsettings.Development.json",optional: true,reloadOnChange: true)
        .AddEnvironmentVariables();

string EnvironmentSettings = configuration.GetSection("Environment").Value;  // arquivo raiz do appsettings.json
AppSettingsRecord settings = new AppSettingsRecord();
new ConfigureFromConfigurationOptions<AppSettingsRecord>(configuration.GetSection(EnvironmentSettings)).Configure(settings);      //configuration.GetSection(EnvironmentSettings).Bind(settings);
settings.CopyToStaticSettings();


// -------------------------- DB --------------------------

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);
RepoDb.PostgreSqlBootstrap.Initialize();
RepoDb.SqlServerBootstrap.Initialize();


// -------------------------- GERAL --------------------------


//AppContext.SetSwitch("System.Globalization.Invariant",true);


//builder.Services.AddCors(options => {
//    options.AddPolicy("AllowAll",builder =>
//         builder.AllowAnyOrigin()
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .SetIsOriginAllowed(origin => true)
//                //.AllowCredentials()
//                .WithHeaders(HeaderNames.ContentType,HeaderNames.Authorization,"x-custom-header")
//        );
//});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll",builder =>
         builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
        );
});


builder.Services.AddCors(options => {
    options.AddPolicy("AllowSites",builder =>
         builder.WithOrigins("https://jacaptei.com.br",
                             "https://homolog.jacaptei.com.br",
                             "https://homolog-admin.jacaptei.com.br/",
                             "https://localhost:2240"
                             )
                            .AllowAnyMethod()
                            .AllowAnyHeader()
        );
});





// ------------------------ JWT .NET ------------------------------


//var key = Encoding.ASCII.GetBytes(Settings.key);
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Config.settings.key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    x.Events = new JwtBearerEvents {
        OnChallenge = context => {
            // Call this to skip the default logic and avoid using the default response
            context.HandleResponse();
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";// and here also.
            var result = new AppReturn();
            result.SetAsForbidden();
            //var result = JsonSerializer.Serialize(context);
            //var result = context;
            //return context.Response.WriteAsync("AREA_RESTRITA---NECESSARIA_AUTENTICACAO");
            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        },
        OnForbidden = context => {
            context.Response.ContentType = "application/json";// and here also.
            var result = new AppReturn();
            context.Response.StatusCode = 403;
            result.SetAsForbidden();
            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

    };


});

builder.Services.AddServices(settings);

// -----------------------------------------------------

var app = builder.Build();

app.UseHttpsRedirection();
//app.UseDefaultFiles();

//var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
//provider.Mappings[".vue"] = "application/javascript"; // NOTE: add the extension (with period) and its type
//app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
//app.UseStaticFiles();


app.UseRouting();

app.UseCors("AllowAll");
//app.UseCors("AllowSites");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(name: "inArea",pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(name: "inArea",pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
//app.MapControllers();

//app.MapDefaultControllerRoute();
//app.UseEndpoints(endpoints =>{    endpoints.MapControllerRoute(        name: "default",        pattern: "{controller=home}/{action=index}");});


app.Run();

