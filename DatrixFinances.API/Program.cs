using System.Diagnostics.CodeAnalysis;
using System.Text;
using DatrixFinances.API.Repositories;
using DatrixFinances.API.Services;
using DatrixFinances.API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#if RELEASE
using System.Security.Cryptography.X509Certificates;
#endif

namespace DatrixFinances.API;

[ExcludeFromCodeCoverage]
public partial class Program
{

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var launchProfile = Environment.GetEnvironmentVariable("LAUNCH_PROFILE")
                                ?? builder.Configuration["launchProfile"]
                                ?? builder.Environment.EnvironmentName;
#if RELEASE
        var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var certs = store.Certificates.Find(
            X509FindType.FindByThumbprint,
            "51831bebd217e11a8be341cb2715cb7aad2633a5",
            false);
        var cert = certs.Count > 0 ? certs[0] : null;
        store.Close();

        if (cert == null)
            throw new Exception("Certificate not found in store.");

        builder.WebHost.ConfigureKestrel(options =>
        {
            var port = string.Equals(launchProfile, "testing", StringComparison.OrdinalIgnoreCase) ? 7684 : 7985;

            options.ListenAnyIP(port, listenOptions =>
            {
                listenOptions.UseHttps(cert);
            });
        });
#endif
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        builder.Services.AddAuthentication("Bearer").AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, SimpleBearerHandler>("Bearer", null);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                options.SerializerSettings.Converters.Add(new DateOnlyJsonConverter());
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .AddXmlSerializerFormatters();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenNewtonsoftSupport();
        builder.Services.AddSwaggerGen(config =>
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            config.IncludeXmlComments(xmlPath);
            if (!string.Equals(launchProfile, "testing", StringComparison.OrdinalIgnoreCase) && !string.Equals(launchProfile, "development", StringComparison.OrdinalIgnoreCase))
            {
                config.SwaggerDoc("DatrixFinances", new OpenApiInfo
                {
                    Title = "Datrix Finances",
                    Version = "Production"
                });
            }
            else
            {
                config.SwaggerDoc("DatrixFinances", new OpenApiInfo
                {
                    Title = "Datrix Finances",
                    Version = "Testing"
                });
            }

            config.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/oauth2/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>()
                    }
                }
            });

            config.OperationFilter<AuthOperationFilter>();
        });

        builder.Services.AddDbContext<DatabaseContext>();

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IXMLService, XMLService>();
        builder.Services.AddScoped<IYukiService, YukiService>();

        builder.Services.AddScoped<IAPIActivityAspectRepository, APIActivityAspectRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddHttpClient("Yuki", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Urls:yuki"] ?? string.Empty);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        if (!string.Equals(launchProfile, "testing", StringComparison.OrdinalIgnoreCase) && !string.Equals(launchProfile, "development", StringComparison.OrdinalIgnoreCase))
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddRazorPages();
        builder.Services.AddMudServices();
        builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(o => { o.DetailedErrors = true; });

        builder.WebHost.UseStaticWebAssets();

        var app = builder.Build();

        app.UseSession();

        app.UseMiddleware<Middleware>();

        app.UseSwagger(config => config.RouteTemplate = "swagger/{documentName}.json");
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/DatrixFinances.json", "Datrix Finances");
            options.DefaultModelsExpandDepth(-1);
            options.InjectJavascript("/swagger/mandatory-headers.js");
        });

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Database.Migrate();
        }

        app.MapGet("/login", async context =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/login.html");
        });

        app.MapPost("/login", async context =>
        {
            var form = await context.Request.ReadFormAsync();
            var username = form["username"];
            var password = form["password"];

            if (username == builder.Configuration["Authentication:Swagger:Username"] && password == builder.Configuration["Authentication:Swagger:Password"])
            {
                context.Session.SetString("Swagger-Authentication-Login-Token-24fdsjq_1!-32421:AdFGQdjvDSQEFkmqfQGQZEDfgmgqdf$ùsdf==", "true");
                context.Response.Redirect("/portal");
            }
            else
            {
                await context.Response.SendFileAsync("wwwroot/login.html");
            }
        });

        app.UseStaticFiles();

        app.UseHttpsRedirection();
        
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapBlazorHub();
        app.MapStaticAssets();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }

}