

using JaCaptei.UI.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.StaticFiles;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:52230","https://localhost:52240");

//var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
//    ApplicationName = typeof(Program).Assembly.FullName,
//    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
//    //WebRootPath = "root",
//    Args = args
//});



// Configuration

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

configuration.SetBasePath(environment.ContentRootPath)
        .AddJsonFile("appsettings.json",               optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.Development.json",   optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.Production.json",    optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

string EnvironmentSettings = configuration.GetSection("Environment").Value;  // arquivo raiz do appsettings.json
AppSettingsRecord settings = new AppSettingsRecord();
new ConfigureFromConfigurationOptions<AppSettingsRecord>(configuration.GetSection(EnvironmentSettings)).Configure(settings);      //configuration.GetSection(EnvironmentSettings).Bind(settings);
settings.CopyToStaticSettings();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);

//Config.settings.obs = "teste";

//builder.Services.AddEndpointsApiExplorer();  // to mvc application 


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});


builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//builder.Services.AddMvc().WithRazorPagesRoot("/");
builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

//app.UseMiddleware<SpaRouteMiddleware>();

// Para usar arquivos estaticos somente (driblar Asp.Net Pages Route) - sempre colocar antes de app.UseStaticFiles  --https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0
//app.UseDefaultFiles(); 

app.UseStaticFiles();


app.UseRouting();

//app.MapGet("/home",() => "/");
//app.MapGet("/sobre",() => "/");


//app.UseCors(x => x
//            .AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            //.AllowCredentials()
//            .SetIsOriginAllowed(origin => true)
//            .WithHeaders(HeaderNames.ContentType,HeaderNames.Authorization,"x-custom-header")
// );

app.UseCors("AllowAll");

//app.UseAuthentication();
//app.UseAuthorization();
app.MapControllerRoute(name: "default"  ,pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapDefaultControllerRoute();
app.MapControllers();


/*
app.UseEndpoints(endpoints =>{
    endpoints.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
    endpoints.MapFallbackToPage("/");
});
*/



app.Run();

/*
public class SpaRouteMiddleware {

    private readonly RequestDelegate next;

    // You can inject a dependency here that gives you access
    // to your ignored route configuration.
    public SpaRouteMiddleware(RequestDelegate next) {
        this.next = next;
    }

    public async Task Invoke(HttpContext context) {
       
        if(context.Request.Path.HasValue && !context.Request.Path.Value.Contains("#")) {
            //context.Response.StatusCode = 404;
            //Console.WriteLine("Ignored!");
            // return;
            var newPath = context.Request.Host.Value + "/#" + context.Request.Path.Value;
            //context.Response.Redirect(newPath,true,true);
            context.Response.Redirect(newPath);
            //return;
        }

       // if(context.Request.Path.Value != "/#") {
            //context.Response.Redirect("/#/",true,true);
            //context.Request.Path = "/";
            //return;
        //}
        
        await next.Invoke(context);
    }

}


*/